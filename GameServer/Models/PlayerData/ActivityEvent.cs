using System;
using System.ComponentModel.DataAnnotations;

namespace GameServer.Models.PlayerData
{
    public class ActivityEvent
    {
        [Key]
        public int Id { get; set; }
        public int AuthorId { get; set; }
        public int PlayerId { get; set; }
        public int PlayerCreationId { get; set; }
        public DateTime CreatedAt { get; set; }
        public ActivityList List { get; set; }
        public ActivityType Type { get; set; }
        public string Topic { get; set; }
        public string Description { get; set; }
        public string ImageURL { get; set; }
        public string ImageMD5 { get; set; }
        public string Tags { get; set; }
        public string Subject { get; set; }
        public string AllusionType { get; set; }
        public int AllusionId { get; set; }
    }
}
