using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameServer.Models.PlayerData
{
    public class Analytics
    {
        [Key]
        public int Id { get; set; }
        
        public int UserId { get; set; }
        
        [ForeignKey(nameof(UserId))]
        public User User { get; set; }
        
        public string Data { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime CreatedAt { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime UpdatedAt { get; set; }
    }
}
