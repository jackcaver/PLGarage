using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace GameServer.Models.PlayerData.PlayerCreations
{
    public class PlayerCreationReview
    {
        public string Content { get; set; }
        public int Id { get; set; }
        public int PlayerCreationId { get; set; }

        [ForeignKey(nameof(PlayerCreationId))]
        public PlayerCreationData Creation { get; set; }

        public string PlayerCreationName => Creation.Name;
        public string PlayerCreationUsername => Creation.Author.Username;
        public string PlayerCreationAssociatedItemIds => Creation.AssociatedItemIds;
        public int PlayerCreationLevelMode => Creation.LevelMode;
        public bool PlayerCreationIsTeamPick => Creation.IsTeamPick;
        public int PlayerId { get; set; }

        [ForeignKey(nameof(PlayerId))]
        public User User { get; set; }

        public List<PlayerCreationReviewRatingData> ReviewRatings { get; set; }

        public int RatingDown => ReviewRatings.Count(match => match.Type == RatingType.BOO);
        public int RatingUp => ReviewRatings.Count(match => match.Type == RatingType.YAY);
        public string Username => User.Username;
        public string Tags { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime CreatedAt { get; set; }

        public bool IsRatedByMe(int id)
        {
            return ReviewRatings.Any(match => match.PlayerId == id);
        }

        public bool IsMine(int id)
        {
            return PlayerId == id;
        }
    }
}
