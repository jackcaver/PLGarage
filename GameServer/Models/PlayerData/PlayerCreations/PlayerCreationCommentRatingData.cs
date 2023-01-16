using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameServer.Models.PlayerData.PlayerCreations
{
    public class PlayerCreationCommentRatingData
    {
        public int Id { get; set; }
        public int PlayerId { get; set; }

        [ForeignKey(nameof(PlayerId))]
        public User Player { get; set; }

        public DateTime RatedAt { get; set; }
        public int PlayerCreationCommentId { get; set; }

        [ForeignKey(nameof(PlayerCreationCommentId))]
        public PlayerCreationCommentData Comment { get; set; }

        public RatingType Type { get; set; }
    }
}
