using System.Collections.Generic;
using System.Xml.Serialization;

namespace GameServer.Models.Response
{
    public class SubLeaderboardPlayer 
    {
        [XmlAttribute("created_at")]
        public string CreatedAt { get; set; }
        [XmlAttribute("id")]
        public int Id { get; set; }
        [XmlAttribute("platform")]
        public string Platform { get; set; }
        [XmlAttribute("player_id")]
        public int PlayerId { get; set; }
        [XmlAttribute("playgroup_size")]
        public int PlaygroupSize { get; set; }
        [XmlAttribute("rank")]
        public int Rank { get; set; }
        [XmlAttribute("sub_group_id")]
        public int SubGroupId { get; set; }
        [XmlAttribute("sub_key_id")]
        public int SubKeyId { get; set; }
        [XmlAttribute("updated_at")]
        public string UpdatedAt { get; set; }
        [XmlAttribute("username")]
        public string Username { get; set; }
        [XmlAttribute("score")]
        public float Score { get; set; }
        [XmlAttribute("finish_time")]
        public float FinishTime { get; set; }
        //MNR
        [XmlAttribute("best_lap_time")]
        public float BestLapTime { get; set; }
        [XmlAttribute("character_idx")]
        public int CharacterIdx { get; set; }
        [XmlAttribute("ghost_car_data_md5")]
        public string GhostCarDataMd5 { get; set; }
        [XmlAttribute("kart_idx")]
        public int KartIdx { get; set; }
        [XmlAttribute("skill_level_id")]
        public int SkillLevelId { get; set; }
        [XmlAttribute("skill_level_name")]
        public string SkillLevelName { get; set; }
    }

    [XmlType(TypeName = "leaderboard")]    // TODO: Can we change this to XmlRoot?
    public class SubLeaderboard
    {
        [XmlAttribute("page")]
        public int Page { get; set; }
        [XmlAttribute("row_end")]
        public int RowEnd { get; set; }
        [XmlAttribute("row_start")]
        public int RowStart { get; set; }
        [XmlAttribute("sub_group_id")]
        public int SubGroupId { get; set; }
        [XmlAttribute("sub_key_id")]
        public int SubKeyId { get; set; }
        [XmlAttribute("playgroup_size")]
        public int PlaygroupSize { get; set; }
        [XmlAttribute("total")]
        public int Total { get; set; }
        [XmlAttribute("total_pages")]
        public int TotalPages { get; set; }
        [XmlAttribute("type")]
        public string Type { get; set; }
        [XmlElement("player")]
        public List<SubLeaderboardPlayer> LeaderboardPlayersList { get; set; }
    }

    public class SubLeaderboardViewResponse {
        [XmlElement("my_stats")]
        public SubLeaderboardPlayer MyStats { get; set; }
        [XmlElement("leaderboard")]
        public SubLeaderboard Leaderboard { get; set; }
    }

    public class SubLeaderboardFriendsViewResponse {
        [XmlElement("my_stats")]
        public SubLeaderboardPlayer MyStats { get; set; }
        [XmlElement("friends_leaderboard")]
        public SubLeaderboard FriendsLeaderboard { get; set; }
    }
}