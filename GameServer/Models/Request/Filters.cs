using GameServer.Models.GameBrowser;
using GameServer.Models.PlayerData;
using GameServer.Models.PlayerData.PlayerCreations;

namespace GameServer.Models.Request
{
    public class Filters
    {
        public string[] id { get; set; }
        public string[] player_id { get; set; }
        public string[] username { get; set; }
        public PlayerCreationType player_creation_type { get; set; }
        public string race_type { get; set; }
        public string[] tags { get; set; }
        public bool? is_remixable { get; set; }
        public bool? ai { get; set; }
        public bool? auto_reset { get; set; }
        //MNR PSP game list filters
        public GameState? game_state { get; set; }
        public bool? is_ranked { get; set; }
        public GameType? game_type { get; set; }
        public Platform? platform { get; set; }
        public string track_group { get; set; }
        public string privacy { get; set; }
        public string speed_class { get; set; }
        public int? track { get; set; }
        public int? number_laps { get; set; }
    }
}
