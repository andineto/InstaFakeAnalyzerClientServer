namespace InstaFakeAnalyzer.DTOs
{
    public class InstagramWebhookDTOs
    {
        public string Object { get; set; }
        public List<Entry> Entry { get; set; }
    }

    public class Entry
    {
        public string Id { get; set; }
        public long Time { get; set; }
        public List<Messaging> Messaging { get; set; }
    }

    public class Messaging
    {
        public User Sender { get; set; }
        public User Recipient { get; set; }
        public long Timestamp { get; set; }
        public Message Message { get; set; }
    }

    public class User
    {
        public string Id { get; set; }
    }

    public class Message
    {
        public string Mid { get; set; }
        public string Text { get; set; }
    }
}
