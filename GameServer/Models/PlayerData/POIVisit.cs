using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;

namespace GameServer.Models.PlayerData
{
    public class POIVisit
    {
        [Key]
        public int Id { get; set; }
        public int PlayerId { get; set; }

        [ForeignKey(nameof(PlayerId))]
        public User Player { get; set; }

        public int PointOfInterestId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
