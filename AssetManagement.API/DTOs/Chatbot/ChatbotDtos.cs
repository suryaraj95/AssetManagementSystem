namespace AssetManagement.API.DTOs.Chatbot;

public record ChatMessageDto(string Role, string Content);
public record ChatRequestDto(string Message, List<ChatMessageDto> History);
public record ChatResponseDto(string Reply);
