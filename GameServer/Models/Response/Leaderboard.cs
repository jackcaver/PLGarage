using System.Collections.Generic;
using System.Xml.Serialization;

namespace GameServer.Models.Response
{
    public class LeaderboardPlayer 
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
    }

    [XmlType(TypeName = "leaderboard")]
    public class Leaderboard
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
        public List<LeaderboardPlayer> LeaderboardPlayersList { get; set; }
    }

    public class SubLeaderboardViewResponse {
        public LeaderboardPlayer my_stats { get; set; }
        public Leaderboard leaderboard { get; set; }
    }

    public class SubLeaderboardFriendsViewResponse {
        public LeaderboardPlayer my_stats { get; set; }
        public Leaderboard friends_leaderboard { get; set; }
    }
}