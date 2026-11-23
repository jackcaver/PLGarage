using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameServer.Models.PlayerData.PlayerCreations
{
    public class PlayerCreationDownload
    {
        [Key]
        public int Id { get; set; }
        public int PlayerId { get; set; }
        
        [ForeignKey(nameof(PlayerId))]
        public User Player { get; set; }
        
        public int PlayerCreationId { get; set; }

        [ForeignKey(nameof(PlayerCreationId))]
        public PlayerCreationData Creation { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime DownloadedAt { get; set; }
    }
}
