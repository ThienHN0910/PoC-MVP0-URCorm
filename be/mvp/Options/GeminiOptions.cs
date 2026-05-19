namespace mvp.Options;

public sealed class GeminiOptions
{
    public const string SectionName = "Gemini";

    public string ApiKey { get; set; } = string.Empty;

    public string Model { get; set; } = "gemini-3.1-flash-lite";

    public string Prompt { get; set; } = "Bạn là một trợ lý quản lý khách sạn chuyên nghiệp. Hãy đọc review sau đây và sinh ra 3 câu trả lời khác nhau bằng tiếng Việt. Yêu cầu bắt buộc: Trả về một chuỗi JSON duy nhất, KHÔNG chứa ký tự markdown (như ```json).";
}
