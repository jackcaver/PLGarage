namespace GameServer.Models.Request
{
    public class PlayerEvent
    {
        public string type { get; set; }
        public string player_creation_id { get; set; }
        public string player_id { get; set; }
        public string description { get; set; }
    }
}
