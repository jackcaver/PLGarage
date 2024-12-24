using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;
using GameServer.Models.Common;

namespace GameServer.Models.PlayerData.PlayerCreations
{
    public class PlayerCreationPoint
    {
        [Key]
        public int Id { get; set; }
        public Platform Platform { get; set; }
        public PlayerCreationType Type { get; set; }
        public int Amount { get; set; }
        public DateTime CreatedAt { get; set; }

        public User Player { get; set; }
        public PlayerCreationData Creation { get; set; }
    }
}
