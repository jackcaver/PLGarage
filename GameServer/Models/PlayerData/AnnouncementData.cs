using GameServer.Models.Common;
using System;
using System.ComponentModel.DataAnnotations;

namespace GameServer.Models.PlayerData
{
    public class AnnouncementData
    {
        [Key]
        public int Id { get; set; }
        public Platform Platform { get; set; }
        public string LanguageCode { get; set; }
        public string Subject { get; set; }
        public string Text { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
