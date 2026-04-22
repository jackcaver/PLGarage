using System;
using System.Collections.Generic;

namespace GameServer.Models.Response
{
    public class HotlapSnapshotDto
    {
        public HotlapTrackDto track { get; set; }
        public int resetInSeconds { get; set; }
        public List<HotlapTimeDto> topTimes { get; set; }
    }

    public class HotlapLeaderboardDto
    {
        public HotlapTrackDto track { get; set; }
        public List<HotlapTimeDto> entries { get; set; }
    }

    public class HotlapTrackDto
    {
        public int id { get; set; }
        public string name { get; set; }
        public string rating { get; set; }
        public int? creatorId { get; set; }
        public string creatorUsername { get; set; }
    }

    public class HotlapUserDto
    {
        public int id { get; set; }
        public string username { get; set; }
    }

    public class HotlapTimeDto
    {
        public int rank { get; set; }
        public int playerId { get; set; }
        public string playerUsername { get; set; }
        public float bestLapTime { get; set; }
        public DateTime updatedAt { get; set; }
    }
}