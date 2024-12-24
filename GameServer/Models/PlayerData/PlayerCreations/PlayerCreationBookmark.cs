using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameServer.Models.PlayerData.PlayerCreations
{
    public class PlayerCreationBookmark
    {
        [Key]
        public int Id { get; set; }
        public DateTime BookmarkedAt { get; set; }

        public User User { get; set; }
        public PlayerCreationData BookmarkedCreation { get; set; }
    }
}
