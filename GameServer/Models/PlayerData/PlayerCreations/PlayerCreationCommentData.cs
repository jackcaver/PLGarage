using GameServer.Models.Common;
using GameServer.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace GameServer.Models.PlayerData.PlayerCreations
{
    public class PlayerCreationCommentData
    {
        [Key]
        public int Id { get; set; }
        public Platform Platform { get; set; }
        public string Body { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public User Player { get; set; }
        public PlayerCreationData Creation { get; set; }
        public ICollection<PlayerCreationCommentRatingData> Ratings { get; set; }
    }
}
