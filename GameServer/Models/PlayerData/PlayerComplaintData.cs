using EntityFrameworkCore.Projectables;
using GameServer.Models.Moderation;
using GameServer.Models.Request;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace GameServer.Models.PlayerData
{
    public class PlayerComplaintData
    {
        public int Id { get; set; }
        public int UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        [JsonIgnore]
        public User User { get; set; }
        public string AuthorUsername => User.Username;
        public int PlayerId { get; set; }

        [ForeignKey(nameof(PlayerId))]
        [JsonIgnore]
        public User Player { get; set; }
        public string Username => Player.Username;
        [JsonConverter(typeof(StringEnumConverter))]
        public PlayerComplaintReason Reason { get; set; }
        public string Comments { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public ReportState State { get; set; }
        
        [JsonIgnore]
        public List<ModeratorAssignment> ModeratorAssignments { get; set; }
        
        [Projectable]
        public List<int> AssignedModerators => ModeratorAssignments != null ? ModeratorAssignments.Select(a => a.UserId).ToList() : new();
        
        [Projectable]
        public bool IsAssigned(int id) => ModeratorAssignments.Any(match => match.UserId == id);
    }
}
