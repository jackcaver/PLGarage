namespace GameServer.Models.Request
{
    public class Game 
    {
        public string game_type { get; set; }
        public int track_idx { get; set; }
        public string game_state { get; set; }
        public int host_player_id { get; set; }
        public string platform { get; set; }
        public string name { get; set; }
        public bool is_ranked { get; set; }
    }
}