using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameServer.Models.PlayerData
{
    public class PlayerCommentRatingData
    {
        public int Id { get; set; }
        public int PlayerId { get; set; }

        [ForeignKey(nameof(PlayerId))]
        public User Player { get; set; }

        public DateTime RatedAt { get; set; }
        public int PlayerCommentId { get; set; }

        [ForeignKey(nameof(PlayerCommentId))]
        public PlayerCommentData Comment { get; set; }

        public RatingType Type { get; set; }
    }
}
