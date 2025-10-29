namespace GameServer.Models.Moderation
{
    public class MinimalUserInfo
    {
        public int ID { get; set; }
        public string Username { get; set; }
        public bool PlayedMNR { get; set; }
        public bool IsPSNLinked { get; set; }
        public bool IsRPCNLinked { get; set; }
        public bool ShowCreationsWithoutPreviews { get; set; }
        public bool AllowOppositePlatform { get; set; }
    }
}
