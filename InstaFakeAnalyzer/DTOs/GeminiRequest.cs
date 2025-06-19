// DTOs/GeminiRequest.cs
namespace InstaFakeAnalyzer.DTOs
{
    public class GeminiRequest
    {
        public GeminiContent[] contents { get; set; }
    }

    public class GeminiContent
    {
        public GeminiPart[] parts { get; set; }
    }

    public class GeminiPart
    {
        public string text { get; set; }
    }
}
