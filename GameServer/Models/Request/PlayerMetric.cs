namespace GameServer.Models.Request
{
    public class PlayerMetric
    {
        public string data { get; set; }
        public float points { get; set; }
        public float volatility { get; set; }
        public float deviation { get; set; }
        public int num_games { get; set; }
    }
}
