using GameServer.Models.Moderation;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameServer.Models.PlayerData
{
    public class ModeratorAssignment
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        
        [JsonIgnore]
        [ForeignKey(nameof(UserId))]
        public Moderator User { get; set; }
        
        public AssignmentType Type { get; set; }
        
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? GriefReportId { get; set; }
        
        [JsonIgnore]
        [ForeignKey(nameof(GriefReportId))]
        public GriefReportData Report { get; set; }
        
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? PlayerComplaintId { get; set; }
        
        [JsonIgnore]
        [ForeignKey(nameof(PlayerComplaintId))]
        public PlayerComplaintData PlayerComplaint { get; set; }
        
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? PlayerCreationComplaintId { get; set; }
        
        [JsonIgnore]
        [ForeignKey(nameof(PlayerCreationComplaintId))]
        public PlayerCreationComplaintData PlayerCreationComplaint { get; set; }
    }
}
