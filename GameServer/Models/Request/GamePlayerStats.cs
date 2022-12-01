namespace GameServer.Models.Request
{
    public class GamePlayerStats 
    {
        public bool is_complete { get; set; }
        public int stat_1 { get; set; }
        public int stat_2 { get; set; }
        public float score { get; set; }
        public int is_winner { get; set; }
        public int finish_place { get; set; }
        public float finish_time { get; set; }
        public int laps_completed { get; set; }
        public float points { get; set; }
        public float volatility { get; set; }
        public float deviation { get; set; }
        public int playgroup_size { get; set; }
        public int num_kills { get; set; }
    }
}