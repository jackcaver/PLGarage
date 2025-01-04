using System.Collections.Generic;
using System.Xml.Serialization;

namespace GameServer.Models.Response
{
    public class PersonalSubLeaderboardPlayer
    {
        [XmlAttribute("username")]
        public string Username { get; set; }
        [XmlAttribute("player_id")]
        public int PlayerId { get; set; }
        [XmlAttribute("rank")]
        public int Rank { get; set; }
        [XmlAttribute("best_lap_time")]
        public float BestLapTime { get; set; }
        [XmlAttribute("kart_idx")]
        public int KartIdx { get; set; }
        [XmlAttribute("character_idx")]
        public int CharacterIdx { get; set; }
        [XmlAttribute("track_idx")]
        public int TrackIdx { get; set; }
        [XmlAttribute("sub_key_id")]
        public int SubKeyId { get; set; }
        [XmlAttribute("skill_level_id")]
        public int SkillLevelId { get; set; }
        [XmlAttribute("location_tag")]
        public string LocationTag { get; set; }
        [XmlAttribute("latitude")]
        public float Latitude { get; set; }
        [XmlAttribute("longitude")]
        public float Longitude { get; set; }
    }

    public class PersonalSubLeaderboard
    {
        [XmlAttribute("total_pages")]
        public int TotalPages { get; set; }
        [XmlAttribute("page")]
        public int Page { get; set; }
        [XmlAttribute("total")]
        public int Total { get; set; }
        [XmlElement("player")]
        public List<PersonalSubLeaderboardPlayer> Scores { get; set; } 
    }

    public class SubLeaderboardPersonalViewResponse
    {
        [XmlElement("my_stats")]
        public PersonalSubLeaderboardPlayer MyStats { get; set; }
        [XmlElement("leaderboard")]
        public PersonalSubLeaderboard Leaderboard { get; set; }
    }
}
