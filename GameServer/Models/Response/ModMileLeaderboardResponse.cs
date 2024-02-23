using System.Collections.Generic;
using System.Xml.Serialization;

namespace GameServer.Models.Response
{
    public class ModMileLeaderboardStat
    {
        [XmlAttribute]
        public int rank { get; set; }
        [XmlAttribute]
        public string player { get; set; }
        [XmlAttribute]
        public int visits { get; set; }
        [XmlAttribute]
        public int travel_points { get; set; }
        [XmlAttribute]
        public string destination { get; set; }
        [XmlAttribute]
        public string city { get; set; }
        [XmlAttribute]
        public string country { get; set; }
    }

    public class ModMileLeaderboard
    {
        [XmlAttribute]
        public int total_pages { get; set; }
        [XmlAttribute]
        public int page { get; set; }
        [XmlAttribute]
        public int total { get; set; }
        [XmlElement("leaderboard_stats")]
        public List<ModMileLeaderboardStat> Scores { get; set; }
    }

    public class CitiesLeaderboardResponse
    {
        public ModMileLeaderboardStat player_stats { get; set; }
        public ModMileLeaderboard cities_leaderboard { get; set; }
    }

    public class DestinationsLeaderboardResponse
    {
        public ModMileLeaderboardStat player_stats { get; set; }
        public ModMileLeaderboard destinations_leaderboard { get; set; }
    }

    public class PlayersLeaderboardResponse
    {
        public ModMileLeaderboardStat player_stats { get; set; }
        public ModMileLeaderboard players_leaderboard { get; set; }
    }
}
