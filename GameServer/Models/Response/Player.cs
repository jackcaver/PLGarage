using System.Xml.Serialization;

namespace GameServer.Models.Response
{
    public class player
    {
        [XmlAttribute]
        public int id { get; set; }
        [XmlAttribute]
        public string username { get; set; }
        [XmlAttribute]
        public int hearts { get; set; }
        [XmlAttribute]
        public string presence { get; set; }
        [XmlAttribute]
        public int player_creation_quota { get; set; }
        [XmlAttribute]
        public string created_at { get; set; }
        [XmlAttribute]
        public string updated_at { get; set; }
        [XmlAttribute]
        public string quote { get; set; }
        [XmlAttribute]
        public string city { get; set; }
        [XmlAttribute]
        public string state { get; set; }
        [XmlAttribute]
        public string province { get; set; }
        [XmlAttribute]
        public string country { get; set; }
        [XmlAttribute]
        public bool hearted_by_me { get; set; }
        [XmlAttribute]
        public int total_tracks { get; set; }
        [XmlAttribute]
        public int rank { get; set; }
        [XmlAttribute]
        public int points { get; set; }
        [XmlAttribute]
        public int online_races { get; set; }
        [XmlAttribute]
        public int online_wins { get; set; }
        [XmlAttribute]
        public int online_finished { get; set; }
        [XmlAttribute]
        public int online_forfeit { get; set; }
        [XmlAttribute]
        public int online_disconnected { get; set; }
        [XmlAttribute]
        public int win_streak { get; set; }
        [XmlAttribute]
        public int longest_win_streak { get; set; }
        [XmlAttribute]
        public int online_races_this_week { get; set; }
        [XmlAttribute]
        public int online_wins_this_week { get; set; }
        [XmlAttribute]
        public int online_finished_this_week { get; set; }
        [XmlAttribute]
        public int online_races_last_week { get; set; }
        [XmlAttribute]
        public int online_wins_last_week { get; set; }
        [XmlAttribute]
        public int online_finished_last_week { get; set; }
    }
}