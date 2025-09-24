using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameServer.Models.PlayerData
{
    public class HeartedProfile
    {
        [Key]
        public int Id { get; set; }
        public int UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public User User { get; set; }

        public int HeartedUserId { get; set; }

        [ForeignKey(nameof(HeartedUserId))]
        public User HeartedUser { get; set; }

        public DateTime HeartedAt { get; set; }
        public bool IsMNR { get; set; }

        public int Hearts => HeartedUser.Hearts;
        public string Quote => HeartedUser.Quote;
        public int TotalTracks => HeartedUser.TotalTracks;
        public string Username => HeartedUser.Username;
        public bool IsHeartedByMe(int id, bool IsMNR) => HeartedUser.IsHeartedByMe(id, IsMNR);
    }
}
