using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace GameServer.Models.PlayerData
{
    public class AwardUnlock
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }

        public User Player { get; set; }
    }
}
