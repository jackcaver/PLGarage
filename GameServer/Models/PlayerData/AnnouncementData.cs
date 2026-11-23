using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameServer.Models.PlayerData
{
    public class AnnouncementData
    {
        public int Id { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime CreatedAt { get; set; }
        public string LanguageCode { get; set; }
        public string Subject { get; set; }
        public string Text { get; set; }
        public Platform Platform { get; set; }
    }
}
