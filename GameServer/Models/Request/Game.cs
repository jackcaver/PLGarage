using GameServer.Models.PlayerData;
using GameServer.Models.PlayerData.Games;

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
    }
}