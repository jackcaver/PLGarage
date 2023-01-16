using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameServer.Models.PlayerData.PlayerCreations
{
    public class PlayerCreationReviewRatingData
    {
        public int Id { get; set; }
        public int PlayerId { get; set; }

        [ForeignKey(nameof(PlayerId))]
        public User Player { get; set; }

        public DateTime RatedAt { get; set; }
        public int PlayerCreationReviewId { get; set; }

        [ForeignKey(nameof(PlayerCreationReviewId))]
        public PlayerCreationReview Review { get; set; }

        public RatingType Type { get; set; }
    }
}
