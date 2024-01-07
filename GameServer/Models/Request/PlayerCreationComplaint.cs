using Microsoft.AspNetCore.Http;

namespace GameServer.Models.Request
{
    public class PlayerCreationComplaint
    {
        public int owner_player_id { get; set; }
        public int player_creation_id { get; set; }
        public PlayerComplaintReason player_complaint_reason { get; set; }
        public string player_comments { get; set; }
        public IFormFile preview { get; set; }
    }
}
