using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameServer.Models.PlayerData
{
    public class HeartedProfile
    {
        [Key]
        public int Id { get; set; }
        public bool IsMNR { get; set; }
        public DateTime HeartedAt { get; set; }

        public User User { get; set; }
        public User HeartedUser { get; set; }
    }
}
