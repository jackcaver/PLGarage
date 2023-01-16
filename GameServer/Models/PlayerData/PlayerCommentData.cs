using GameServer.Utils;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace GameServer.Models.PlayerData
{
    public class PlayerCommentData
    {
        private Database _database;
        private Database database
        {
            get
            {
                if (_database != null) return _database;
                return _database = new Database();
            }
            set => _database = value;
        }

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
        public string AuthorUsername => this.database.Users.FirstOrDefault(match => match.UserId == this.AuthorId).Username;
        public string Username => this.database.Users.FirstOrDefault(match => match.UserId == this.PlayerId).Username;
        public int RatingUp => this.database.PlayerCommentRatings.Count(match => match.PlayerCommentId == this.Id && match.Type == RatingType.YAY);
        public int RatingDown => this.database.PlayerCommentRatings.Count(match => match.PlayerCommentId == this.Id && match.Type == RatingType.BOO);

        public bool IsRatedByMe(int id)
        {
            var entry = this.database.PlayerCommentRatings.FirstOrDefault(match => match.PlayerCommentId == this.Id && match.PlayerId == id);
            return entry != null;
        }
    }
}
