using EntityFrameworkCore.Projectables;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace GameServer.Models.PlayerData
{
    public class PlayerCommentData
    {
        public string Body { get; set; }
        public DateTime CreatedAt { get; set; }
        public int Id { get; set; }
        public Platform Platform { get; set; }
        public int AuthorId { get; set; }

        [ForeignKey(nameof(AuthorId))]
        public User Author { get; set; }

        public int PlayerId { get; set; }

        [ForeignKey(nameof(PlayerId))]
        public User Player { get; set; }

        public DateTime UpdatedAt { get; set; }
        [Projectable]
        public string AuthorUsername => Author.Username;
        [Projectable]
        public string Username => Player.Username;

        public List<PlayerCommentRatingData> CommentRating { get; set; }
        [Projectable]
        public int RatingUp => CommentRating.Count(match => match.Type == RatingType.YAY);
        [Projectable]
        public int RatingDown => CommentRating.Count(match => match.Type == RatingType.BOO);
        [Projectable]
        public bool IsRatedByMe(int id) => CommentRating.Any(match => match.PlayerId == id);
    }
}
