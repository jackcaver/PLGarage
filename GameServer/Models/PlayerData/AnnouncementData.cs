using System;

namespace GameServer.Models.PlayerData
{
    public class AnnouncementData
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public string LanguageCode { get; set; }
        public string Subject { get; set; }
        public string Text { get; set; }
        public Platform Platform { get; set; }
    }
}
