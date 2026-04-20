using EntityFrameworkCore.Projectables;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace GameServer.Models.PlayerData.PlayerCreations
{
    public class PlayerCreationCommentData
    {
        public string Body { get; set; }
        public DateTime CreatedAt { get; set; }
        public int Id { get; set; }
        public Platform Platform { get; set; }
        public int PlayerCreationId { get; set; }

        [ForeignKey(nameof(PlayerCreationId))]
        public PlayerCreationData Creation { get; set; }

        public int PlayerId { get; set; }

        [ForeignKey(nameof(PlayerId))]
        public User Player { get; set; }

        public DateTime UpdatedAt { get; set; }
        [Projectable]
        public string Username => Player.Username;

        public List<PlayerCreationCommentRatingData> CommentRatings { get; set; }

        [Projectable]
        public int RatingUp => CommentRatings.Count(match => match.Type == RatingType.YAY);
        [Projectable]
        public int RatingDown => CommentRatings.Count(match => match.Type == RatingType.BOO);
        [Projectable]
        public bool IsRatedByMe(int id) => CommentRatings.Any(match => match.PlayerId == id);
        
    }
}
