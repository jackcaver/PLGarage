using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameServer.Models.PlayerData.Games
{
    public class GamePlayerData
    {
        [Key]
        public int Id { get; set; }
        public int GameId { get; set; }

        [ForeignKey(nameof(GameId))]
        public GameData Game { get; set; }

        public int PlayerId { get; set; }
        public int TeamId { get; set; }
        public GameState GameState { get; set; }
    }
}
