using GameServer.Utils;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace GameServer.Models.PlayerData.PlayerCreations
{
    public class PlayerCreationReview
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

        public string Content { get; set; }
        public int Id { get; set; }
        public int PlayerCreationId { get; set; }

        [ForeignKey(nameof(PlayerCreationId))]
        public PlayerCreationData Creation { get; set; }

        public string PlayerCreationName => this.database.PlayerCreations.FirstOrDefault(match => match.PlayerCreationId == this.PlayerCreationId).Name;
        public string PlayerCreationUsername => this.database.PlayerCreations.Include(x => x.Author).FirstOrDefault(match => match.PlayerCreationId == this.PlayerCreationId).Author.Username;
        public string PlayerCreationAssociatedItemIds => this.database.PlayerCreations.FirstOrDefault(match => match.PlayerCreationId == this.PlayerCreationId).AssociatedItemIds;
        public int PlayerCreationLevelMode => this.database.PlayerCreations.FirstOrDefault(match => match.PlayerCreationId == this.PlayerCreationId).LevelMode;
        public bool PlayerCreationIsTeamPick => this.database.PlayerCreations.FirstOrDefault(match => match.PlayerCreationId == this.PlayerCreationId).IsTeamPick;
        public int PlayerId { get; set; }

        [ForeignKey(nameof(PlayerId))]
        public User User { get; set; }

        public int RatingDown => this.database.PlayerCreationReviewRatings.Count(match => match.PlayerCreationReviewId == this.Id && match.Type == RatingType.BOO);
        public int RatingUp => this.database.PlayerCreationReviewRatings.Count(match => match.PlayerCreationReviewId == this.Id && match.Type == RatingType.YAY);
        public string Username => this.database.Users.FirstOrDefault(match => match.UserId == this.PlayerId).Username;
        public string Tags { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime CreatedAt { get; set; }

        public bool IsRatedByMe(int id)
        {
            var entry = this.database.PlayerCreationReviewRatings.FirstOrDefault(match => match.PlayerCreationReviewId == this.Id && match.PlayerId == id);
            return entry != null;
        }

        public bool IsMine(int id)
        {
            return this.PlayerId == id;
        }
    }
}
