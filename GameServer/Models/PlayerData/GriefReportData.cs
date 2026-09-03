using EntityFrameworkCore.Projectables;
using GameServer.Models.Moderation;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace GameServer.Models.PlayerData
{
    public class GriefReportData
    {
        [Key]
        public int Id { get; set; }
        public int UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        [JsonIgnore]
        public User User { get; set; }

        public string Context { get; set; }
        public string Reason { get; set; }
        public string Comments { get; set; }
        public string BadRectTop { get; set; }
        public string BadRectBottom { get; set; }
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
