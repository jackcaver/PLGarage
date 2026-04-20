using EntityFrameworkCore.Projectables;
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

        [Projectable]
        public string PlayerCreationName => Creation.Name;
        [Projectable]
        public string PlayerCreationUsername => Creation.Author.Username;
        [Projectable]
        public string PlayerCreationAssociatedItemIds => Creation.AssociatedItemIds;
        [Projectable]
        public int PlayerCreationLevelMode => Creation.LevelMode;
        [Projectable]
        public bool PlayerCreationIsTeamPick => Creation.IsTeamPick;
        public int PlayerId { get; set; }

        [ForeignKey(nameof(PlayerId))]
        public User User { get; set; }

        public List<PlayerCreationReviewRatingData> ReviewRatings { get; set; }

        [Projectable]
        public int RatingDown => ReviewRatings.Count(match => match.Type == RatingType.BOO);
        [Projectable]
        public int RatingUp => ReviewRatings.Count(match => match.Type == RatingType.YAY);
        [Projectable]
        public string Username => User.Username;
        public string Tags { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime CreatedAt { get; set; }
        [Projectable]
        public bool IsRatedByMe(int id) => ReviewRatings.Any(match => match.PlayerId == id);
        [Projectable]
        public bool IsMine(int id) => PlayerId == id;
    }
}
