﻿using GameServer.Models.Common;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameServer.Models.PlayerData.PlayerCreations
{
    public class PlayerCreationCommentRatingData
    {
        public int Id { get; set; }
        public RatingType Type { get; set; }
        public DateTime RatedAt { get; set; }

        public User Player { get; set; }
        public PlayerCreationCommentData Comment { get; set; }
    }
}
