using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameServer.Models.PlayerData.PlayerCreations
{
    public class HeartedPlayerCreation
    {
        [Key]
        public int Id { get; set; }
        public int UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public User User { get; set; }

        public int HeartedPlayerCreationId { get; set; }

        [ForeignKey(nameof(HeartedPlayerCreationId))]
        public PlayerCreationData HeartedCreation { get; set; }

        public DateTime HeartedAt { get; set; }
    }
}
