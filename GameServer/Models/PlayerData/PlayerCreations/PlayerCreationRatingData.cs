using GameServer.Models.Common;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameServer.Models.PlayerData.PlayerCreations
{
    public class PlayerCreationRatingData
    {
        [Key]
        public int Id { get; set; }
        public RatingType Type { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
        public DateTime RatedAt { get; set; }

        public User Player { get; set; }
        public PlayerCreationData Creation { get; set; }
    }
}
