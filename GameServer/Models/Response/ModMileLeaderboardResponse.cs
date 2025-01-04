using System.Collections.Generic;
using System.Xml.Serialization;

namespace GameServer.Models.Response
{
    public class ModMileLeaderboardStat
    {
        [XmlAttribute("rank")]
        public int Rank { get; set; }
        [XmlAttribute("player")]
        public string Player { get; set; }
        [XmlAttribute("visits")]
        public int Visits { get; set; }
        [XmlAttribute("travel_points")]
        public int TravelPoints { get; set; }
        [XmlAttribute("destination")]
        public string Destination { get; set; }
        [XmlAttribute("city")]
        public string City { get; set; }
        [XmlAttribute("country")]
        public string Country { get; set; }
    }

    public class ModMileLeaderboard
    {
        [XmlAttribute("total_pages")]
        public int TotalPages { get; set; }
        [XmlAttribute("page")]
        public int Page { get; set; }
        [XmlAttribute("total")]
        public int Total { get; set; }
        [XmlElement("leaderboard_stats")]
        public List<ModMileLeaderboardStat> Scores { get; set; }
    }

    public class CitiesLeaderboardResponse
    {
        [XmlElement("player_stats")]
        public ModMileLeaderboardStat PlayerStats { get; set; }
        [XmlElement("cities_leaderboard")]
        public ModMileLeaderboard CitiesLeaderboard { get; set; }
    }

    public class DestinationsLeaderboardResponse
    {
        [XmlElement("player_stats")]
        public ModMileLeaderboardStat PlayerStats { get; set; }
        [XmlElement("destinations_leaderboard")]
        public ModMileLeaderboard DestinationsLeaderboard { get; set; }
    }

    public class PlayersLeaderboardResponse
    {
        [XmlElement("player_stats")]
        public ModMileLeaderboardStat PlayerStats { get; set; }
        [XmlElement("players_leaderboard")]
        public ModMileLeaderboard PlayersLeaderboard { get; set; }
    }
}
