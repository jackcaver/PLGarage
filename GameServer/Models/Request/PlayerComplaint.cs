namespace GameServer.Models.Request
{
    public class PlayerComplaint
    {
        public int player_id { get; set; }
        public PlayerComplaintReason player_complaint_reason { get; set; }
        public string player_comments { get; set; }
    }
}
