namespace GameServer.Models.Request
{
    public class PlayerCreationRating
    {
        public int player_creation_id { get; set; }
        public int rating { get; set; }
        public string comments { get; set; }
    }
}
