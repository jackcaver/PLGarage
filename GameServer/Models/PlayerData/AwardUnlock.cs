using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace GameServer.Models.PlayerData
{
    public class AwardUnlock
    {
        [Key]
        public int Id { get; set; }
        public int PlayerId { get; set; }

        [ForeignKey(nameof(PlayerId))]
        public User Player { get; set; }

        public string Name { get; set; }
    }
}
