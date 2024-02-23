using GameServer.Models.PlayerData.PlayerCreations;
using GameServer.Models.Request;
using GameServer.Utils;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace GameServer.Models.PlayerData
{
    public class Score
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

        public DateTime CreatedAt { get; set; }
        public int Id { get; set; }
        public Platform Platform { get; set; }
        public int PlayerId { get; set; }

        [ForeignKey(nameof(PlayerId))]
        public User User { get; set; }

        public int PlaygroupSize { get; set; }
        public int SubGroupId { get; set; }
        public int SubKeyId { get; set; }

        [ForeignKey(nameof(SubKeyId))]
        public PlayerCreationData Creation { get; set; }

        public DateTime UpdatedAt { get; set; }
        public string Username => this.database.Users.FirstOrDefault(match => match.UserId == this.PlayerId).Username;
        public float Points { get; set; }
        public float FinishTime { get; set; }
        //MNR
        public bool IsMNR { get; set; }
        public float BestLapTime { get; set; }
        public int CharacterIdx { get; set; }
        public string GhostCarDataMD5 { get; set; }
        public int KartIdx { get; set; }
        //MNR: Road Trip
        public float Latitude { get; set; }
        public float Longitude { get; set; }
        public string LocationTag { get; set; }

        public int GetRank(SortColumn sortColumn)
        {
            var scores = this.database.Scores.Where(match => match.SubKeyId == this.SubKeyId
                && match.SubGroupId == this.SubGroupId
                && match.Platform == this.Platform
                && match.PlaygroupSize == this.PlaygroupSize).ToList();
            if (sortColumn == SortColumn.finish_time)
                scores.Sort((curr, prev) => curr.FinishTime.CompareTo(prev.FinishTime));
            if (sortColumn == SortColumn.score)
                scores.Sort((curr, prev) => prev.Points.CompareTo(curr.Points));
            if (sortColumn == SortColumn.best_lap_time)
                scores.Sort((curr, prev) => curr.BestLapTime.CompareTo(prev.BestLapTime));
            return scores.FindIndex(match => match == this)+1;
        }
    }
}
