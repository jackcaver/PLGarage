using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameServer.Models.PlayerData.PlayerCreations
{
    public class HeartedPlayerCreation
    {
        [Key]
        public int Id { get; set; }
        public DateTime HeartedAt { get; set; }

        public User User { get; set; }
        public PlayerCreationData HeartedCreation { get; set; }
    }
}
