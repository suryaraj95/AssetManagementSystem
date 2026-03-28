using AssetManagement.API.DTOs.Chatbot;
using AssetManagement.API.Services;

namespace AssetManagement.API.Endpoints;

public static class ChatbotEndpoints
{
    public static void MapChatbotEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/chatbot")
            .RequireAuthorization(policy => policy.RequireRole("Admin"));

        group.MapPost("/chat", async (ChatRequestDto dto, ChatbotService chatbotService) =>
        {
            if (string.IsNullOrWhiteSpace(dto.Message))
                return Results.BadRequest(new { error = "Message cannot be empty." });

            var reply = await chatbotService.ChatAsync(dto);
            return Results.Ok(new ChatResponseDto(reply));
        });
    }
}
