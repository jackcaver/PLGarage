using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;

namespace GameServer.Models.PlayerData.PlayerCreations
{
    public class PlayerCreationRaceStarted
    {
        [Key]
        public int Id { get; set; }
        public int PlayerCreationId { get; set; }

        [ForeignKey(nameof(PlayerCreationId))]
        public PlayerCreationData Creation { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime StartedAt { get; set; }
    }
}
