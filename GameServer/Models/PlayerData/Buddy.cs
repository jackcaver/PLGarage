using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameServer.Models.PlayerData
{
    public class Buddy
    {
        [Key]
        public int Id { get; set; }
        
        public int UserId { get; set; }
        
        [ForeignKey(nameof(UserId))]
        public User User { get; set; }
        
        public int BuddyUserId { get; set; }
        
        [ForeignKey(nameof(BuddyUserId))]
        public User BuddyUser { get; set; }
    }
}
