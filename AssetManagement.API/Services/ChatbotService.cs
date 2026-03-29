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

    private const int MaxRetries = 3;

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
        // 1. Resolve API Key (Priority: Env Var > appsettings.json)
        var apiKey = Environment.GetEnvironmentVariable("GROQ_API_KEY") 
                     ?? _config["Groq:ApiKey"];
        
        var model  = _config["Groq:Model"] ?? "llama3-70b-8192";
        var apiUrl = _config["Groq:ApiUrl"] ?? "https://api.groq.com/openai/v1/chat/completions";

        if (string.IsNullOrWhiteSpace(apiKey) || apiKey.StartsWith("YOUR_"))
            return "⚠️ Groq API key is not configured. Please set the `GROQ_API_KEY` environment variable or update `appsettings.json`.";

        // ── Step 1: Intent Detection ────────────────────────────────────────
        var intentSystemPrompt = """
            You are an Intent Classifier for an Asset Management System.
            Given a user's message, you must respond ONLY with a JSON object.
            
            JSON Schema:
            {
              "intent": "string", 
              "parameters": { "paramName": "value" },
              "is_data_query": boolean,
              "general_answer": "string"
            }
            
            Rules:
            1. If the user asks for DATA (counts, lists, details, history, warranty), set:
               is_data_query: true, intent: [one of the following], parameters: [extracted values]
               Available intents: get_asset_summary, get_assets_by_status, get_employees_without_assets, get_employees_with_assets, 
                                  get_assets_by_employee, get_assets_by_category, get_assets_expiring_warranty, get_assets_by_branch, 
                                  get_assets_by_condition, get_pending_requests, get_asset_details, get_recent_asset_history, 
                                  get_assets_purchased_in_range, get_high_value_assets.
            2. If the user asks a GENERAL question (system help, "how are you?", "what can you do?"), or something unrelated, set:
               is_data_query: false, intent: "general_query", general_answer: "Your helpful response here."
            3. If the user asks about something NOT related to asset management, respond with is_data_query: false and 
               general_answer: "I can only answer questions related to asset management data in this platform."
            4. Respond ONLY with the JSON object. Do not include markdown blocks or any other text.
            """;

        var intentPayload = new JsonObject
        {
            ["model"] = model,
            ["temperature"] = 0,
            ["messages"] = new JsonArray
            {
                new JsonObject { ["role"] = "system", ["content"] = intentSystemPrompt },
                new JsonObject { ["role"] = "user", ["content"] = request.Message }
            }
        };

        var (intentResponse, intentError) = await CallGroqAsync(apiKey, apiUrl, intentPayload);
        if (intentResponse == null) return intentError ?? "❌ Failed to detect intent via Groq.";

        var intentJsonRaw = ExtractGroqContent(intentResponse);
        if (string.IsNullOrWhiteSpace(intentJsonRaw)) return "❌ Groq failed to categorize your request.";

        JsonObject? intentJson;
        try 
        {
            var cleanJson = intentJsonRaw.Trim();
            if (cleanJson.StartsWith("```json")) cleanJson = cleanJson.Replace("```json", "").Replace("```", "").Trim();
            else if (cleanJson.StartsWith("```")) cleanJson = cleanJson.Replace("```", "").Trim();
            
            intentJson = JsonNode.Parse(cleanJson)?.AsObject();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to parse Groq intent JSON: {Raw}", intentJsonRaw);
            return "❌ I had trouble understanding the intent of your request. Please try rephrasing.";
        }

        if (intentJson == null) return "❌ Failed to parse intent.";

        var isDataQuery = intentJson["is_data_query"]?.GetValue<bool>() ?? false;
        var intentName  = intentJson["intent"]?.GetValue<string>();
        var genAnswer   = intentJson["general_answer"]?.GetValue<string>();

        if (!isDataQuery)
            return genAnswer ?? "How can I help you today?";

        // ── Step 2: Backend Data Fetching ───────────────────────────────────
        var parameters = intentJson["parameters"]?.Deserialize<JsonElement>() ?? default;
        _logger.LogInformation("Groq Chatbot dispatching intent: {IntentName}", intentName);

        object toolResult;
        try
        {
            toolResult = await _queryService.DispatchToolCallAsync(intentName ?? "", parameters);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Data fetch failed for intent {IntentName}", intentName);
            return $"❌ Sorry, I encountered an error while fetching the data: {ex.Message}";
        }

        // ── Step 3: Natural Language Response ───────────────────────────────
        var formatPrompt = $"""
            You are a helpful Asset Management Assistant.
            User asked: "{request.Message}"
            Real technical data from the database: {JsonSerializer.Serialize(toolResult, _jsonOpts)}
            
            Task: Provide a clear, human-friendly answer based ONLY on the data provided. 
            Keep it concise but helpful. Use bullet points or tables where appropriate.
            """;

        var formatPayload = new JsonObject
        {
            ["model"] = model,
            ["messages"] = new JsonArray
            {
                new JsonObject { ["role"] = "system", ["content"] = "You are a helpful Asset Management Assistant. Answer based on the provided JSON data." },
                new JsonObject { ["role"] = "user", ["content"] = formatPrompt }
            }
        };

        var (finalResponse, finalError) = await CallGroqAsync(apiKey, apiUrl, formatPayload);
        if (finalResponse == null) 
            return finalError ?? "❌ I fetched the data but failed to format the response.";

        return ExtractGroqContent(finalResponse) ?? "I have the data but was unable to generate a text response.";
    }

    private async Task<(JsonObject? Response, string? Error)> CallGroqAsync(string apiKey, string url, JsonObject payload)
    {
        for (int attempt = 0; attempt < MaxRetries; attempt++)
        {
            using var req = new HttpRequestMessage(HttpMethod.Post, url);
            req.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", apiKey);
            req.Content = new StringContent(payload.ToJsonString(), Encoding.UTF8, "application/json");

            HttpResponseMessage resp;
            try
            {
                resp = await _http.SendAsync(req);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Groq HTTP call failed on attempt {Attempt}", attempt + 1);
                if (attempt == MaxRetries - 1)
                    return (null, $"Network error reaching Groq API: {ex.Message}");
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
                    _logger.LogError(ex, "Failed to parse Groq response JSON");
                    return (null, "❌ Received an unreadable response from Groq.");
                }
            }

            _logger.LogWarning("Groq returned {Status}: {Body}", resp.StatusCode, raw);
            string groqMsg = raw;
            try
            {
                var errNode = JsonNode.Parse(raw);
                groqMsg = errNode?["error"]?["message"]?.GetValue<string>() ?? raw;
            }
            catch { }

            var friendlyMsg = (int)resp.StatusCode switch
            {
                400 => $"❌ Groq rejected the request: {groqMsg}",
                401 or 403 => "❌ Groq API key is invalid or unauthorized.",
                429 => "⏳ Groq rate limit reached. Please wait a moment.",
                503 => "🔧 Groq service is temporarily unavailable.",
                _ => $"❌ Groq returned HTTP {(int)resp.StatusCode}: {groqMsg}"
            };

            if (resp.StatusCode == System.Net.HttpStatusCode.TooManyRequests && attempt < MaxRetries - 1)
            {
                await Task.Delay(TimeSpan.FromSeconds(5 * (attempt + 1))); 
                continue;
            }

            return (null, friendlyMsg);
        }

        return (null, "⏳ Groq rate limit reached after retries.");
    }

    private static string? ExtractGroqContent(JsonObject response)
    {
        try
        {
            return response["choices"]?[0]?["message"]?["content"]?.GetValue<string>();
        }
        catch { return null; }
    }
}
