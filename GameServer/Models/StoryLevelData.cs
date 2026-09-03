using GameServer.Models.PlayerData;
using GameServer.Models.PlayerData.PlayerCreations;

namespace GameServer.Models
{
    public class StoryLevelData
    {
        public string Name { get; set; }
        public int ScoreboardMode { get; set; }
        public RaceType RaceType { get; set; }
        public Platform Platform { get; set; }
        public bool IsMNR { get; set; }
        public int MaxHumans { get; set; }
        public bool AutoReset { get; set; }
    }
}
