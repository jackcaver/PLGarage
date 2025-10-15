﻿using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;

namespace GameServer.Models.PlayerData
{
    public class PlayerExperiencePoint
    {
        [Key]
        public int Id { get; set; }
        public int PlayerId { get; set; }

        [ForeignKey(nameof(PlayerId))]
        public User Player { get; set; }

        public DateTime CreatedAt { get; set; }
        public int Amount { get; set; }

        public Platform Platform { get; set; } = Platform.PS3;
    }
}
