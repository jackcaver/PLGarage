using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameServer.Models.PlayerData.Games
{
    public class GameData
    {
        [Key]
        public int Id { get; set; }
        public GameType GameType { get; set; }
        public int TrackIdx { get; set; }
        public GameState GameState { get; set; }
        public int HostPlayerId { get; set; }

        [ForeignKey(nameof(HostPlayerId))]
        public User User { get; set; }

        public Platform Platform { get; set; }
        public string Name { get; set; }
        public bool IsRanked { get; set; }
        //MNR
        public string SpeedClass { get; set; }
        public int Track { get; set; }
        public string TrackGroup { get; set; }
        public string Privacy { get; set; }
        public int NumberLaps { get; set; }
    }
}
