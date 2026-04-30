using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameServer.Models.PlayerData
{
    public class ModeratorSession
    {
        [Key]
        [JsonIgnore]
        public int Id { get; set; }
        public int UserId { get; set; }
        
        [JsonIgnore]
        [ForeignKey(nameof(UserId))]
        public Moderator User { get; set; }
        
        public DateTime LastTokenRefresh { get; set; }
        
        public Guid SessionId { get; set; }
    }
}
