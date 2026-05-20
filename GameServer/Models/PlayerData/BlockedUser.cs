using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameServer.Models.PlayerData
{
    public class BlockedUser
    {
        [Key]
        public int Id { get; set; }
        
        public int UserId { get; set; }
        
        [ForeignKey(nameof(UserId))]
        public User User { get; set; }
        
        public int BlockedUserId { get; set; }
        
        [ForeignKey(nameof(BlockedUserId))]
        public User BlockedPlayer { get; set; }
    }
}
