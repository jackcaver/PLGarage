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
        public string AuthorUsername => Author.Username;
        public string Username => Player.Username;

        public List<PlayerCommentRatingData> CommentRating { get; set; }
        public int RatingUp => CommentRating.Count(match => match.Type == RatingType.YAY);
        public int RatingDown => CommentRating.Count(match => match.Type == RatingType.BOO);

        public bool IsRatedByMe(int id)
        {
            return CommentRating.Any(match => match.PlayerId == id);
        }
    }
}
