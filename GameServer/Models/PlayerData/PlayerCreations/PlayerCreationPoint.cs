using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;

namespace GameServer.Models.PlayerData.PlayerCreations
{
    public class PlayerCreationPoint
    {
        [Key]
        public int Id { get; set; }
        public int PlayerCreationId { get; set; }

        [ForeignKey(nameof(PlayerCreationId))]
        public PlayerCreationData Creation { get; set; }

        public int PlayerId { get; set; }

        [ForeignKey(nameof(PlayerId))]
        public User Player { get; set; }

        public Platform Platform { get; set; }
        public PlayerCreationType Type { get; set; }
        public DateTime CreatedAt { get; set; }
        public int Amount { get; set; }
    }
}
