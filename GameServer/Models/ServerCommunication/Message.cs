namespace GameServer.Models.ServerCommunication
{
    public class Message
    {
        public string Type { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public string Content { get; set; }
    }
}
