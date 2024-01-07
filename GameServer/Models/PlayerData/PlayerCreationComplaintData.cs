using GameServer.Models.PlayerData.PlayerCreations;
using GameServer.Models.Request;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameServer.Models.PlayerData
{
    public class PlayerCreationComplaintData
    {
        public int Id { get; set; }
        public int UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public User User { get; set; }

        public int PlayerId { get; set; }

        [ForeignKey(nameof(PlayerId))]
        public User Player { get; set; }

        public int PlayerCreationId { get; set; }

        [ForeignKey(nameof(PlayerCreationId))]
        public PlayerCreationData PlayerCreation { get; set; }

        public PlayerComplaintReason Reason { get; set; }
        public string Comments { get; set; }
    }
}
