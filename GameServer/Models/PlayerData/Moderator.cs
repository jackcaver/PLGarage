using System.ComponentModel.DataAnnotations;

namespace GameServer.Models.PlayerData
{
    public class Moderator
    {
        [Key]
        public int ID { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
