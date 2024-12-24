using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameServer.Models.PlayerData.PlayerCreations
{
    public class PlayerCreationView
    {
        [Key]
        public int Id { get; set; }
        public DateTime ViewedAt { get; set; }

        public PlayerCreationData Creation { get; set; }
    }
}
