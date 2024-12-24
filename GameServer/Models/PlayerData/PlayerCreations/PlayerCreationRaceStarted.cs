using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;

namespace GameServer.Models.PlayerData.PlayerCreations
{
    public class PlayerCreationRaceStarted
    {
        [Key]
        public int Id { get; set; }
        public DateTime StartedAt { get; set; }

        public PlayerCreationData Creation { get; set; }
    }
}
