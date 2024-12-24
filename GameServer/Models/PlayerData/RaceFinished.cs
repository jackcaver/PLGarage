using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;

namespace GameServer.Models.PlayerData
{
    public class RaceFinished
    {
        [Key]
        public int Id { get; set; }
        public bool IsWinner { get; set; }
        public DateTime FinishedAt { get; set; }

        public User Player { get; set; }
    }
}
