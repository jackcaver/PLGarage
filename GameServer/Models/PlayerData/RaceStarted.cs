using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameServer.Models.PlayerData
{
    public class RaceStarted
    {
        [Key]
        public int Id { get; set; }
        public DateTime StartedAt { get; set; }

        public User Player { get; set; }
    }
}
