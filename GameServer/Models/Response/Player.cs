using System.Xml.Serialization;

namespace GameServer.Models.Response
{
    [XmlType(TypeName = "player")]    // TODO: Can we change this to XmlRoot?
    public class PlayerProfileResponse
    {
        [XmlAttribute("id")]
        public int Id { get; set; }
        [XmlAttribute("username")]
        public string Username { get; set; }
        [XmlAttribute("hearts")]
        public int Hearts { get; set; }
        [XmlAttribute("presence")]
        public string Presence { get; set; }
        [XmlAttribute("player_creation_quota")]
        public int PlayerCreationQuota { get; set; }
        [XmlAttribute("created_at")]
        public string CreatedAt { get; set; }
        [XmlAttribute("updated_at")]
        public string UpdatedAt { get; set; }
        [XmlAttribute("quote")]
        public string Quote { get; set; }
        [XmlAttribute("city")]
        public string City { get; set; }
        [XmlAttribute("state")]
        public string State { get; set; }
        [XmlAttribute("province")]
        public string Province { get; set; }
        [XmlAttribute("country")]
        public string Country { get; set; }
        [XmlAttribute("hearted_by_me")]
        public bool HeartedByMe { get; set; }
        [XmlAttribute("total_tracks")]
        public int TotalTracks { get; set; }
        [XmlAttribute("rank")]
        public int Rank { get; set; }
        [XmlAttribute("points")]
        public int Points { get; set; }
        [XmlAttribute("online_races")]
        public int OnlineRaces { get; set; }
        [XmlAttribute("online_wins")]
        public int OnlineWins { get; set; }
        [XmlAttribute("online_finished")]
        public int OnlineFinished { get; set; }
        [XmlAttribute("online_forfeit")]
        public int OnlineForfeit { get; set; }
        [XmlAttribute("online_disconnected")]
        public int OnlineDisconnected { get; set; }
        [XmlAttribute("win_streak")]
        public int WinStreak { get; set; }
        [XmlAttribute("longest_win_streak")]
        public int LongestWinStreak { get; set; }
        [XmlAttribute("online_races_this_week")]
        public int OnlineRacesThisWeek { get; set; }
        [XmlAttribute("online_wins_this_week")]
        public int OnlineWinsThisWeek { get; set; }
        [XmlAttribute("online_finished_this_week")]
        public int OnlineFinishedThisWeek { get; set; }
        [XmlAttribute("online_races_last_week")]
        public int OnlineRacesLastWeek { get; set; }
        [XmlAttribute("online_wins_last_week")]
        public int OnlineWinsLastWeek { get; set; }
        [XmlAttribute("online_finished_last_week")]
        public int OnlineFinishedLastWeek { get; set; }

        //MNR
        [XmlAttribute("total_characters")]
        public int TotalCharacters { get; set; }
        [XmlAttribute("total_karts")]
        public int TotalKarts { get; set; }
        [XmlAttribute("total_player_creations")]
        public int TotalPlayerCreations { get; set; }
        [XmlAttribute("creator_points")]
        public float CreatorPoints { get; set; }
        [XmlAttribute("creator_points_last_week")]
        public float CreatorPointsLastWeek { get; set; }
        [XmlAttribute("creator_points_this_week")]
        public float CreatorPointsThisWeek { get; set; }
        [XmlAttribute("experience_points")]
        public float ExperiencePoints { get; set; }
        [XmlAttribute("experience_points_last_week")]
        public float ExperiencePointsLastWeek { get; set; }
        [XmlAttribute("experience_points_this_week")]
        public float ExperiencePointsThisWeek { get; set; }
        [XmlAttribute("longest_drift")]
        public float LongestDrift { get; set; }
        [XmlAttribute("longest_hang_time")]
        public string LongestHangTime { get; set; }
        [XmlAttribute("player_id")]
        public int PlayerId { get; set; }
        [XmlAttribute("skill_level")]
        public string SkillLevel { get; set; }
        [XmlAttribute("skill_level_id")]
        public int SkillLevelId { get; set; }
        [XmlAttribute("skill_level_name")]
        public string SkillLevelName { get; set; }
        [XmlAttribute("star_rating")]
        public string StarRating { get; set; }
        [XmlAttribute("rating")]
        public string Rating { get; set; }
    }
}