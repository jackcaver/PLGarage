using GameServer.Utils;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace GameServer.Models.PlayerData.PlayerCreations
{
    public class PlayerCreationCommentData
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
        public int PlayerCreationId { get; set; }

        [ForeignKey(nameof(PlayerCreationId))]
        public PlayerCreationData Creation { get; set; }

        public int PlayerId { get; set; }

        [ForeignKey(nameof(PlayerId))]
        public User Player { get; set; }

        public DateTime UpdatedAt { get; set; }
        public string Username => database.Users.FirstOrDefault(match => match.UserId == PlayerId).Username;
        public int RatingUp => this.database.PlayerCreationCommentRatings.Count(match => match.PlayerCreationCommentId == this.Id && match.Type == RatingType.YAY);
        public int RatingDown => this.database.PlayerCreationCommentRatings.Count(match => match.PlayerCreationCommentId == this.Id && match.Type == RatingType.BOO);

        public bool IsRatedByMe(int id)
        {
            var entry = this.database.PlayerCreationCommentRatings.FirstOrDefault(match => match.PlayerCreationCommentId == this.Id && match.PlayerId == id);
            return entry != null;
        }
    }
}
