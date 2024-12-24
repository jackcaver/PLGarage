using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;

namespace GameServer.Models.PlayerData
{
    public class POIVisit
    {
        [Key]
        public int Id { get; set; }
        public int PointOfInterestId { get; set; }  // TODO: Store in DB or keep hardcoded?
        public DateTime CreatedAt { get; set; }

        public User Player { get; set; }
    }
}
