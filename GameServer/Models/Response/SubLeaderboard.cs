using System.Collections.Generic;
using System.Xml.Serialization;

namespace GameServer.Models.Response
{
    public class SubLeaderboardPlayer 
    {
        [XmlAttribute]
        public string created_at { get; set; }
        [XmlAttribute]
        public int id { get; set; }
        [XmlAttribute]
        public string platform { get; set; }
        [XmlAttribute]
        public int player_id { get; set; }
        [XmlAttribute]
        public int playgroup_size { get; set; }
        [XmlAttribute]
        public int rank { get; set; }
        [XmlAttribute]
        public int sub_group_id { get; set; }
        [XmlAttribute]
        public int sub_key_id { get; set; }
        [XmlAttribute]
        public string updated_at { get; set; }
        [XmlAttribute]
        public string username { get; set; }
        [XmlAttribute]
        public float score { get; set; }
        [XmlAttribute]
        public float finish_time { get; set; }
        //MNR
        [XmlAttribute]
        public float best_lap_time { get; set; }
        [XmlAttribute]
        public int character_idx { get; set; }
        [XmlAttribute]
        public string ghost_car_data_md5 { get; set; }
        [XmlAttribute]
        public int kart_idx { get; set; }
        [XmlAttribute]
        public int skill_level_id { get; set; }
        [XmlAttribute]
        public string skill_level_name { get; set; }
    }

    [XmlType(TypeName = "leaderboard")]
    public class SubLeaderboard
    {
        [XmlAttribute]
        public int page { get; set; }
        [XmlAttribute]
        public int row_end { get; set; }
        [XmlAttribute]
        public int row_start { get; set; }
        [XmlAttribute]
        public int sub_group_id { get; set; }
        [XmlAttribute]
        public int sub_key_id { get; set; }
        [XmlAttribute]
        public int playgroup_size { get; set; }
        [XmlAttribute]
        public int total { get; set; }
        [XmlAttribute]
        public int total_pages { get; set; }
        [XmlAttribute]
        public string type { get; set; }
        [XmlElement("player")]
        public List<SubLeaderboardPlayer> LeaderboardPlayersList { get; set; }
    }

    public class SubLeaderboardViewResponse {
        public SubLeaderboardPlayer my_stats { get; set; }
        public SubLeaderboard leaderboard { get; set; }
    }

    public class SubLeaderboardFriendsViewResponse {
        public SubLeaderboardPlayer my_stats { get; set; }
        public SubLeaderboard friends_leaderboard { get; set; }
    }
}