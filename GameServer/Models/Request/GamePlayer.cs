namespace GameServer.Models.Request
{
    public class GamePlayer 
    {
        public int player_id { get; set; }
        public int team_id { get; set; }
        public string game_state { get; set; }
    }
}