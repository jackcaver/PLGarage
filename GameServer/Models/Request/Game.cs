using GameServer.Models.GameBrowser;
using GameServer.Models.PlayerData;

namespace GameServer.Models.Request
{
    public class Game 
    {
        public GameType game_type { get; set; }
        public int track_idx { get; set; }
        public GameState game_state { get; set; }
        public int host_player_id { get; set; }
        public Platform platform { get; set; }
        public string name { get; set; }
        public bool is_ranked { get; set; }
        //MNR
        public string speed_class { get; set; }
        public int track { get; set; }
        public string track_group { get; set; }
        public string privacy { get; set; }
        public int number_laps { get; set; }
        //MNR PSP
        public int max_players { get; set; }
        public int min_players { get; set; }
        public int lobby_channel_id { get; set; }
        public string password { get; set; }
    }
}