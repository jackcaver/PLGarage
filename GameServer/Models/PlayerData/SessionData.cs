using EntityFrameworkCore.Projectables;
using GameServer.Utils;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameServer.Models.PlayerData
{
    public class SessionData
    {
        [Key]
        [JsonIgnore]
        public int Id { get; set; }
        public int UserId { get; set; }
        
        [JsonIgnore]
        [ForeignKey(nameof(UserId))]
        public User User { get; set; }

        [Projectable]
        public string Username => User.Username;
        public Guid SessionId { get; set; }
        public Presence Presence { get; set; } = Presence.OFFLINE;
        public DateTime LastPing { get; set; } = TimeUtils.Now;
        public Platform Platform { get; set; }
        public bool IsMNR { get; set; }
    }
}
