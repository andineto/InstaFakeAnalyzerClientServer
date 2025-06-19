namespace InstaFakeAnalyzer.DTOs
{
    // DTOs/GeminiResponse.cs
    public class GeminiResponseCandidate
    {
        public GeminiCandidate[] candidates { get; set; }
    }

    public class GeminiCandidate
    {
        public GeminiContent content { get; set; }
    }

    public class GeminiResponse
    {
        public string? text { get; set; }
        public bool fake { get; set; }
    }
}
