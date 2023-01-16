using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameServer.Models.PlayerData.PlayerCreations
{
    public class PlayerCreationBookmark
    {
        [Key]
        public int Id { get; set; }
        public int UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public User User { get; set; }

        public int BookmarkedPlayerCreationId { get; set; }

        [ForeignKey(nameof(BookmarkedPlayerCreationId))]
        public PlayerCreationData BookmarkedCreation { get; set; }

        public DateTime BookmarkedAt { get; set; }
    }
}
