using System;
using System.Collections.Generic;

namespace GameServer.Models.PlayerData.PlayerCreations
{
    public class ScoreSnapshotDto
    {
        public ScoreTrackDto track { get; set; }
        public string platform { get; set; }
        public int total { get; set; }
        public List<ScoreEntryDto> scores { get; set; }
    }

    public class ScoreTrackDto
    {
        public int id { get; set; }
        public string name { get; set; }
        public string rating { get; set; }
        public int? creatorId { get; set; }
        public string creatorUsername { get; set; }
    }

    public class ScoreEntryDto
    {
        public int rank { get; set; }
        public int playerId { get; set; }
        public string playerUsername { get; set; }
        public float score { get; set; }
        public float bestLapTime { get; set; }
        public float finishTime { get; set; }
        public DateTime updatedAt { get; set; }
    }
}
