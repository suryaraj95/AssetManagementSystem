using AssetManagement.API.DTOs.Chatbot;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace AssetManagement.API.Services;

/// <summary>
/// Orchestrates the Gemini function-calling conversation loop.
/// Flow:
///   1. Send user message + tool declarations to Gemini.
///   2. If Gemini responds with a functionCall → execute via ChatbotQueryService → send functionResponse.
///   3. Return Gemini's final natural-language text reply.
/// </summary>
public class ChatbotService
{
    private readonly HttpClient _http;
    private readonly ChatbotQueryService _queryService;
    private readonly IConfiguration _config;
    private readonly ILogger<ChatbotService> _logger;

    private static readonly JsonSerializerOptions _jsonOpts = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    public ChatbotService(
        HttpClient http,
        ChatbotQueryService queryService,
        IConfiguration config,
        ILogger<ChatbotService> logger)
    {
        _http = http;
        _queryService = queryService;
        _config = config;
        _logger = logger;
    }

    public async Task<string> ChatAsync(ChatRequestDto request)
    {
        var apiKey = _config["Gemini:ApiKey"];
        var model  = _config["Gemini:Model"] ?? "gemini-2.0-flash";

        if (string.IsNullOrWhiteSpace(apiKey) || apiKey.StartsWith("YOUR_"))
            return "⚠️ Gemini API key is not configured. Please add your API key to `appsettings.json` under `Gemini:ApiKey`.";

        // ── System instruction ──────────────────────────────────────────────
        var systemInstruction = new JsonObject
        {
            ["parts"] = new JsonArray
            {
                new JsonObject
                {
                    ["text"] = """
                        You are an AI assistant embedded in an Asset Management System.
                        Your ONLY purpose is to answer questions about the asset data in this platform:
                        assets, employees, branches, categories, conditions, statuses, requests, warranty, purchase info, and asset history.
                        
                        Rules:
                        - ALWAYS use the available tool functions to fetch real data before answering. Do not guess or make up numbers.
                        - If a question is unrelated to asset management (e.g. weather, general knowledge, coding), respond:
                          "I can only answer questions related to asset management data in this platform."
                        - Keep responses concise, clear, and helpful. Use bullet points or tables when listing multiple items.
                        - When users ask about counts or totals, use get_asset_summary first.
                        - Today's date for context: use this when interpreting relative date queries.
                    """
                }
            }
        };

        // ── Tool declarations ───────────────────────────────────────────────
        var tools = new JsonArray
        {
            new JsonObject
            {
                ["function_declarations"] = new JsonArray
                {
                    MakeTool("get_asset_summary",
                        "Get a high-level summary: total assets, assigned, available, under maintenance, retired counts, pending requests, and breakdown by category.",
                        new JsonObject { ["type"] = "object", ["properties"] = new JsonObject() }),
                    MakeTool("get_assets_by_status",
                        "Get all assets filtered by a specific status.",
                        new JsonObject
                        {
                            ["type"] = "object",
                            ["properties"] = new JsonObject
                            {
                                ["status"] = Prop("string", "Status value: Assigned, Available, Under Maintenance, or Retired")
                            },
                            ["required"] = new JsonArray { "status" }
                        }),
                    MakeTool("get_employees_without_assets",
                        "Get all active employees who have no asset currently assigned to them.",
                        new JsonObject { ["type"] = "object", ["properties"] = new JsonObject() }),
                    MakeTool("get_employees_with_assets",
                        "Get all employees who currently have at least one asset assigned, including their asset list.",
                        new JsonObject { ["type"] = "object", ["properties"] = new JsonObject() }),
                    MakeTool("get_assets_by_employee",
                        "Get all assets assigned to a specific employee. Search by name, employee ID, or email.",
                        new JsonObject
                        {
                            ["type"] = "object",
                            ["properties"] = new JsonObject
                            {
                                ["employee_name_or_id"] = Prop("string", "Employee full name, employee ID code, or email")
                            },
                            ["required"] = new JsonArray { "employee_name_or_id" }
                        }),
                    MakeTool("get_assets_by_category",
                        "Get all assets belonging to a specific category (e.g. Laptop, Mobile, Furniture).",
                        new JsonObject
                        {
                            ["type"] = "object",
                            ["properties"] = new JsonObject
                            {
                                ["category_name"] = Prop("string", "Category name (partial match supported)")
                            },
                            ["required"] = new JsonArray { "category_name" }
                        }),
                    MakeTool("get_assets_expiring_warranty",
                        "Get assets whose warranty expires within the next N days (default 30).",
                        new JsonObject
                        {
                            ["type"] = "object",
                            ["properties"] = new JsonObject
                            {
                                ["days"] = Prop("integer", "Number of days from today (default: 30)")
                            }
                        }),
                    MakeTool("get_assets_by_branch",
                        "Get all assets at a specific branch or location.",
                        new JsonObject
                        {
                            ["type"] = "object",
                            ["properties"] = new JsonObject
                            {
                                ["branch_name"] = Prop("string", "Branch or office name (partial match supported)")
                            },
                            ["required"] = new JsonArray { "branch_name" }
                        }),
                    MakeTool("get_assets_by_condition",
                        "Get all assets filtered by physical condition: Good, Fair, Poor, or Broken.",
                        new JsonObject
                        {
                            ["type"] = "object",
                            ["properties"] = new JsonObject
                            {
                                ["condition"] = Prop("string", "Condition: Good, Fair, Poor, or Broken")
                            },
                            ["required"] = new JsonArray { "condition" }
                        }),
                    MakeTool("get_pending_requests",
                        "Get all pending asset requests from employees (new asset or replacement requests awaiting approval).",
                        new JsonObject { ["type"] = "object", ["properties"] = new JsonObject() }),
                    MakeTool("get_asset_details",
                        "Get complete details of a single asset by its Asset ID code.",
                        new JsonObject
                        {
                            ["type"] = "object",
                            ["properties"] = new JsonObject
                            {
                                ["asset_id"] = Prop("string", "The Asset ID code (e.g. LPT-001)")
                            },
                            ["required"] = new JsonArray { "asset_id" }
                        }),
                    MakeTool("get_recent_asset_history",
                        "Get the most recent asset activity log entries (assignments, status changes, dispatches, etc.).",
                        new JsonObject
                        {
                            ["type"] = "object",
                            ["properties"] = new JsonObject
                            {
                                ["limit"] = Prop("integer", "Number of records to return (default: 10, max: 100)")
                            }
                        }),
                    MakeTool("get_assets_purchased_in_range",
                        "Get all assets purchased within a specific date range, with total cost.",
                        new JsonObject
                        {
                            ["type"] = "object",
                            ["properties"] = new JsonObject
                            {
                                ["from_date"] = Prop("string", "Start date in YYYY-MM-DD format"),
                                ["to_date"]   = Prop("string", "End date in YYYY-MM-DD format")
                            },
                            ["required"] = new JsonArray { "from_date", "to_date" }
                        }),
                    MakeTool("get_high_value_assets",
                        "Get all assets whose purchase cost is at or above a specified threshold.",
                        new JsonObject
                        {
                            ["type"] = "object",
                            ["properties"] = new JsonObject
                            {
                                ["min_cost"] = Prop("number", "Minimum purchase cost (e.g. 50000)")
                            },
                            ["required"] = new JsonArray { "min_cost" }
                        })
                }
            }
        };

        // ── Build conversation history ───────────────────────────────────────
        var contents = new JsonArray();

        // Prior turns
        foreach (var msg in request.History)
        {
            contents.Add(new JsonObject
            {
                ["role"] = msg.Role == "assistant" ? "model" : "user",
                ["parts"] = new JsonArray { new JsonObject { ["text"] = msg.Content } }
            });
        }

        // New user message
        contents.Add(new JsonObject
        {
            ["role"] = "user",
            ["parts"] = new JsonArray { new JsonObject { ["text"] = request.Message } }
        });

        // ── First Gemini call ───────────────────────────────────────────────
        var geminiReq = new JsonObject
        {
            ["system_instruction"] = systemInstruction,
            ["tools"]             = tools,
            ["contents"]          = contents
        };

        var (firstResponse, firstError) = await CallGeminiAsync(apiKey, model, geminiReq);
        if (firstResponse == null)
            return firstError ?? "❌ Could not reach the Gemini API. Please verify your API key and network connection.";

        // ── Check for functionCall ──────────────────────────────────────────
        var candidate = firstResponse["candidates"]?[0];
        var parts     = candidate?["content"]?["parts"];

        if (parts == null)
            return ExtractText(firstResponse) ?? "I'm unable to answer that right now.";

        // Check if any part is a functionCall
        JsonObject? funcCallPart = null;
        foreach (var part in parts.AsArray())
        {
            if (part is JsonObject obj && obj.ContainsKey("functionCall"))
            {
                funcCallPart = obj;
                break;
            }
        }

        if (funcCallPart == null)
            return ExtractText(firstResponse) ?? "I'm unable to answer that right now.";

        // ── Execute the tool ────────────────────────────────────────────────
        var funcCall  = funcCallPart["functionCall"]!.AsObject();
        var funcName  = funcCall["name"]?.GetValue<string>() ?? "";
        var funcArgs  = funcCall["args"]?.Deserialize<JsonElement>() ?? default;

        _logger.LogInformation("Chatbot dispatching tool: {FunctionName}", funcName);

        object toolResult;
        try
        {
            toolResult = await _queryService.DispatchToolCallAsync(funcName, funcArgs);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Tool execution failed for {FunctionName}", funcName);
            toolResult = new { error = $"Tool execution failed: {ex.Message}" };
        }

        // ── Build second request with functionResponse ───────────────────────
        // Append model's functionCall turn
        contents.Add(new JsonObject
        {
            ["role"] = "model",
            ["parts"] = new JsonArray { funcCallPart.DeepClone() }
        });

        // Append function result turn
        var toolResultJson = JsonSerializer.SerializeToNode(toolResult, _jsonOpts);
        contents.Add(new JsonObject
        {
            ["role"] = "user",
            ["parts"] = new JsonArray
            {
                new JsonObject
                {
                    ["functionResponse"] = new JsonObject
                    {
                        ["name"]     = funcName,
                        ["response"] = new JsonObject { ["content"] = toolResultJson }
                    }
                }
            }
        });

        var secondReq = new JsonObject
        {
            ["system_instruction"] = systemInstruction.DeepClone(),
            ["tools"]             = tools.DeepClone(),
            ["contents"]          = contents
        };

        var (secondResponse, secondError) = await CallGeminiAsync(apiKey, model, secondReq);
        if (secondResponse == null)
            return secondError ?? "❌ Gemini did not return a final response after tool execution.";

        return ExtractText(secondResponse) ?? "I processed your request but couldn't format a response.";
    }

    // ── Helpers ─────────────────────────────────────────────────────────────

    private const int MaxRetries = 3;

    /// Calls Gemini with exponential backoff on 429 rate-limit responses.
    /// Returns (response, errorMessage). errorMessage is non-null on failure.
    private async Task<(JsonObject? Response, string? Error)> CallGeminiAsync(string apiKey, string model, JsonObject payload)
    {
        var url = $"https://generativelanguage.googleapis.com/v1beta/models/{model}:generateContent?key={apiKey}";

        for (int attempt = 0; attempt < MaxRetries; attempt++)
        {
            // Re-create body each attempt (StringContent is not reusable)
            var body = new StringContent(payload.ToJsonString(), Encoding.UTF8, "application/json");

            HttpResponseMessage resp;
            try
            {
                resp = await _http.PostAsync(url, body);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Gemini HTTP call failed on attempt {Attempt}", attempt + 1);
                if (attempt == MaxRetries - 1)
                    return (null, $"Network error reaching Gemini API: {ex.Message}");
                await Task.Delay(TimeSpan.FromSeconds(Math.Pow(2, attempt)));
                continue;
            }

            var raw = await resp.Content.ReadAsStringAsync();

            if (resp.IsSuccessStatusCode)
            {
                try
                {
                    return (JsonNode.Parse(raw)?.AsObject(), null);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to parse Gemini response JSON");
                    return (null, "❌ Received an unreadable response from Gemini.");
                }
            }

            // 429 = rate limited — wait and retry using Gemini's suggested delay if available
            if ((int)resp.StatusCode == 429 && attempt < MaxRetries - 1)
            {
                // Try to read Gemini's own retryDelay suggestion
                int waitSeconds;
                try
                {
                    var errNode = JsonNode.Parse(raw);
                    var retryDelayStr = errNode?["error"]?["details"]
                        ?.AsArray()
                        .Select(d => d?["retryDelay"]?.GetValue<string>())
                        .FirstOrDefault(s => s != null);
                    // retryDelay is like "26s" — parse it, cap at 30s
                    waitSeconds = retryDelayStr != null && int.TryParse(retryDelayStr.TrimEnd('s'), out var rd)
                        ? Math.Min(rd + 2, 30)
                        : new[] { 5, 15, 30 }[attempt]; // fallback: 5s, 15s, 30s
                }
                catch
                {
                    waitSeconds = new[] { 5, 15, 30 }[attempt];
                }
                _logger.LogWarning("Gemini rate-limited (429) on attempt {Attempt}. Retrying in {Wait}s…", attempt + 1, waitSeconds);
                await Task.Delay(TimeSpan.FromSeconds(waitSeconds));
                continue;
            }

            // Other non-success — extract readable message and return
            _logger.LogWarning("Gemini returned {Status}: {Body}", resp.StatusCode, raw);
            string geminiMsg = raw;
            try
            {
                var errNode = JsonNode.Parse(raw);
                geminiMsg = errNode?["error"]?["message"]?.GetValue<string>() ?? raw;
            }
            catch { /* keep raw */ }

            var friendlyMsg = (int)resp.StatusCode switch
            {
                400 => $"❌ Gemini rejected the request: {geminiMsg}",
                401 or 403 => "❌ Gemini API key is invalid or unauthorized. Please check your API key in `appsettings.json`.",
                429 => "⏳ Gemini rate limit reached after retries. Please wait a moment and try again (free tier: 15 requests/min).",
                503 => "🔧 Gemini service is temporarily unavailable. Please try again in a few seconds.",
                _ => $"❌ Gemini returned HTTP {(int)resp.StatusCode}: {geminiMsg}"
            };

            return (null, friendlyMsg);
        }

        return (null, "⏳ Gemini rate limit reached after retries. Please wait a moment and try again.");
    }

    private static string? ExtractText(JsonObject response)
    {
        try
        {
            var parts = response["candidates"]?[0]?["content"]?["parts"];
            if (parts == null) return null;
            var sb = new StringBuilder();
            foreach (var p in parts.AsArray())
            {
                var text = p?["text"]?.GetValue<string>();
                if (!string.IsNullOrWhiteSpace(text)) sb.AppendLine(text);
            }
            return sb.Length > 0 ? sb.ToString().Trim() : null;
        }
        catch { return null; }
    }

    private static JsonObject MakeTool(string name, string description, JsonObject parameters) =>
        new JsonObject
        {
            ["name"]        = name,
            ["description"] = description,
            ["parameters"]  = parameters
        };

    private static JsonObject Prop(string type, string description) =>
        new JsonObject { ["type"] = type, ["description"] = description };
}
