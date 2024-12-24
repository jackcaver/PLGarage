using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameServer.Models.PlayerData.PlayerCreations
{
    public class PlayerCreationUniqueRacer
    {
        [Key]
        public int Id { get; set; }
        public int Version { get; set; }

        public User User { get; set; }
        public PlayerCreationData Creation { get; set; }
    }
}
