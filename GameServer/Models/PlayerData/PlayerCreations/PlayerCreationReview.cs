using GameServer.Models.Common;
using GameServer.Utils;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace GameServer.Models.PlayerData.PlayerCreations
{
    public class PlayerCreationReview
    {
        [Key]
        public int Id { get; set; }
        public string Content { get; set; }
        public string Tags { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public User User { get; set; }
        public PlayerCreationData Creation { get; set; }
        public ICollection<PlayerCreationReviewRatingData> ReviewRatings { get; set; }
    }
}
