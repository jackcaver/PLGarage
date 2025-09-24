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
        public string Username => User.Username;
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
            using var database = new Database();
            var scores = database.Scores.Where(match => match.SubKeyId == SubKeyId
                && match.SubGroupId == SubGroupId
                && match.Platform == Platform
                && match.PlaygroupSize == PlaygroupSize);

            if (sortColumn == SortColumn.finish_time)
                scores = scores.OrderBy(s => s.FinishTime);
            if (sortColumn == SortColumn.score)
                scores = scores.OrderByDescending(s => s.Points);
            if (sortColumn == SortColumn.best_lap_time)
                scores = scores.OrderBy(s => s.BestLapTime);

            return scores.Select(s => s.Id).ToList().FindIndex(match => match == Id)+1;
        }
    }
}
