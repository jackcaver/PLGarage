using System.Xml.Serialization;

namespace GameServer.Models.Response
{
    [XmlType(TypeName = "player")]
    public class PlayerProfileResponse
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

        //MNR
        [XmlAttribute]
        public int total_characters { get; set; }
        [XmlAttribute]
        public int total_karts { get; set; }
        [XmlAttribute]
        public int total_player_creations { get; set; }
        [XmlAttribute]
        public float creator_points { get; set; }
        [XmlAttribute]
        public float creator_points_last_week { get; set; }
        [XmlAttribute]
        public float creator_points_this_week { get; set; }
        [XmlAttribute]
        public float experience_points { get; set; }
        [XmlAttribute]
        public float experience_points_last_week { get; set; }
        [XmlAttribute]
        public float experience_points_this_week { get; set; }
        [XmlAttribute]
        public float longest_drift { get; set; }
        [XmlAttribute]
        public string longest_hang_time { get; set; }
        [XmlAttribute]
        public int player_id { get; set; }
        [XmlAttribute]
        public string skill_level { get; set; }
        [XmlAttribute]
        public int skill_level_id { get; set; }
        [XmlAttribute]
        public string skill_level_name { get; set; }
        [XmlAttribute]
        public string star_rating { get; set; }
        [XmlAttribute]
        public string rating { get; set; }
    }
}