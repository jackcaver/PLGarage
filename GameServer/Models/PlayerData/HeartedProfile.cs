﻿using System;
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
    }
}
