using System.Collections.Generic;
using System.Xml.Serialization;

namespace GameServer.Models.Response
{
    public class PersonalSubLeaderboardPlayer
    {
        [XmlAttribute]
        public string username { get; set; }
        [XmlAttribute]
        public int player_id { get; set; }
        [XmlAttribute]
        public int rank { get; set; }
        [XmlAttribute]
        public float best_lap_time { get; set; }
        [XmlAttribute]
        public int kart_idx { get; set; }
        [XmlAttribute]
        public int character_idx { get; set; }
        [XmlAttribute]
        public int track_idx { get; set; }
        [XmlAttribute]
        public int sub_key_id { get; set; }
        [XmlAttribute]
        public int skill_level_id { get; set; }
        [XmlAttribute]
        public string location_tag { get; set; }
        [XmlAttribute]
        public float latitude { get; set; }
        [XmlAttribute]
        public float longitude { get; set; }
    }

    public class PersonalSubLeaderboard
    {
        [XmlAttribute]
        public int total_pages { get; set; }
        [XmlAttribute]
        public int page { get; set; }
        [XmlAttribute]
        public int total { get; set; }
        [XmlElement("player")]
        public List<PersonalSubLeaderboardPlayer> Scores { get; set; } 
    }

    public class SubLeaderboardPersonalViewResponse
    {
        public PersonalSubLeaderboardPlayer my_stats { get; set; }
        public PersonalSubLeaderboard leaderboard { get; set; }
    }
}
