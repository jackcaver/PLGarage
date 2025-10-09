using System.Collections.Generic;

namespace GameServer.Models.Config
{
    public class HotLapData
    {
        public int TrackId { get; set; }
        public List<int> Queue { get; set; }
    }
}
