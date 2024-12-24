﻿using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;

namespace GameServer.Models.PlayerData
{
    public class TravelPoint
    {
        [Key]
        public int Id { get; set; }
        public int Amount { get; set; }
        public DateTime CreatedAt { get; set; }

        public User Player { get; set; }
    }
}
