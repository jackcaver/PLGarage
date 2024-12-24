using GameServer.Models.Common;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameServer.Models.PlayerData
{
    public class PlayerCommentRatingData
    {
        [Key]
        public int Id { get; set; }
        public RatingType Type { get; set; }
        public DateTime RatedAt { get; set; }

        public User Player { get; set; }
        public PlayerCommentData Comment { get; set; }
    }
}
