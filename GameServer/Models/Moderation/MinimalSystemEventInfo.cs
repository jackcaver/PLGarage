using System;

namespace GameServer.Models.Moderation
{
    public class MinimalSystemEventInfo
    {
        public int ID { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Topic { get; set; }
        public string Description { get; set; }
        public string ImageURL { get; set; }
    }
}
