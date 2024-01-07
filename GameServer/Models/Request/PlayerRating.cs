namespace GameServer.Models.Request
{
    public class PlayerRating
    {
        public int player_id { get; set; }
        public int rating { get; set; }
        public string comments { get; set; }
    }
}
