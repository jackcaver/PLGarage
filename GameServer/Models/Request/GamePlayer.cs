using GameServer.Models.PlayerData.Games;

namespace GameServer.Models.Request
{
    public class GamePlayer 
    {
        public int player_id { get; set; }
        public int team_id { get; set; }
        public GameState game_state { get; set; }
    }
}