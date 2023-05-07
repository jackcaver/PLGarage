using GameServer.Models.PlayerData.PlayerCreations;

namespace GameServer.Models.Request
{
    public class Filters
    {
        public string[] id { get; set; }
        public string[] username { get; set; }
        public PlayerCreationType player_creation_type { get; set; }
        public string race_type { get; set; }
        public string[] tags { get; set; }
    }
}
