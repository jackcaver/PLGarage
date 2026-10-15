using GameServer.Models.PlayerData.PlayerCreations;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameServer.Models.PlayerData
{
    //TODO: figure out indexing on this table
    //TODO: calculated params
    public class Score
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
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
        //[Projectable]
        public string Username { get; set; }//=> User.Username;
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
    }
}
