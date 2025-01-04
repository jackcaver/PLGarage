using System.Collections.Generic;
using System.Xml.Serialization;

namespace GameServer.Models.Response
{
    public class LeaderboardPlayer
    {
        [XmlAttribute("best_lap_time")]
        public float BestLapTime { get; set; }
        [XmlAttribute("character_idx")]
        public int CharacterIdx { get; set; }
        [XmlAttribute("created_at")]
        public string CreatedAt { get; set; }
        [XmlAttribute("creator_points")]
        public float CreatorPoints { get; set; }
        [XmlAttribute("deviation")]
        public float Deviation { get; set; }
        [XmlAttribute("experience_points")]
        public float ExperiencePoints { get; set; }
        [XmlAttribute("game_end")]
        public string GameEnd { get; set; }
        [XmlAttribute("game_start")]
        public string GameStart { get; set; }
        [XmlAttribute("game_type")]
        public string GameType { get; set; }
        [XmlAttribute("ghost_car_data_md5")]
        public string GhostCarDataMd5 { get; set; }
        [XmlAttribute("id")]
        public int Id { get; set; }
        [XmlAttribute("kart_idx")]
        public int KartIdx { get; set; }
        [XmlAttribute("longest_drift")]
        public float LongestDrift { get; set; }
        [XmlAttribute("longest_hang_time")]
        public float LongestHangTime { get; set; }
        [XmlAttribute("longest_win_streak")]
        public int LongestWinStreak { get; set; }
        [XmlAttribute("online_disconnected")]
        public int OnlineDisconnected { get; set; }
        [XmlAttribute("online_finished")]
        public int OnlineFinished { get; set; }
        [XmlAttribute("online_forfeit")]
        public int OnlineForfeit { get; set; }
        [XmlAttribute("online_quits")]
        public int OnlineQuits { get; set; }
        [XmlAttribute("online_races")]
        public int OnlineRaces { get; set; }
        [XmlAttribute("online_wins")]
        public int OnlineWins { get; set; }
        [XmlAttribute("platform")]
        public string Platform { get; set; }
        [XmlAttribute("player_id")]
        public int PlayerId { get; set; }
        [XmlAttribute("points")]
        public float Points { get; set; }
        [XmlAttribute("rank")]
        public int Rank { get; set; }
        [XmlAttribute("skill_level_id")]
        public int SkillLevelId { get; set; }
        [XmlAttribute("skill_level_name")]
        public string SkillLevelName { get; set; }
        [XmlAttribute("track_idx")]
        public int TrackIdx { get; set; }
        [XmlAttribute("type")]
        public string Type { get; set; }
        [XmlAttribute("updated_at")]
        public string UpdatedAt { get; set; }
        [XmlAttribute("username")]
        public string Username { get; set; }
        [XmlAttribute("volatility")]
        public float Volatility { get; set; }
        [XmlAttribute("win_streak")]
        public int WinStreak { get; set; }
    }

    public class Leaderboard
    {
        [XmlAttribute("game_type")]
        public string GameType { get; set; }
        [XmlAttribute("page")]
        public int Page { get; set; }
        [XmlAttribute("row_end")]
        public int RowEnd { get; set; }
        [XmlAttribute("row_start")]
        public int RowStart { get; set; }
        [XmlAttribute("total")]
        public int Total { get; set; }
        [XmlAttribute("total_pages")]
        public int TotalPages { get; set; }
        [XmlAttribute("type")]
        public string Type { get; set; }
        [XmlElement("player")]
        public List<LeaderboardPlayer> LeaderboardPlayersList { get; set; }
    }

    public class LeaderboardColumn
    {
        [XmlAttribute("display_name")]
        public string DisplayName { get; set; }
        [XmlAttribute("name")]
        public string Name { get; set; }
    }

    public class LeaderboardColumns
    {
        [XmlElement("column")]
        public List<LeaderboardColumn> Columns { get; set; }
    }

    public class LeaderboardViewResponse
    {
        [XmlElement("my_stats")]
        public LeaderboardPlayer MyStats { get; set; }
        [XmlElement("leaderboard")]
        public Leaderboard Leaderboard { get; set; }
        [XmlElement("leaderboard_columns")]
        public LeaderboardColumns LeaderboardColumns { get; set; }
    }

    public class LeaderboardFriendsViewResponse
    {
        [XmlElement("my_stats")]
        public LeaderboardPlayer MyStats { get; set; }
        [XmlElement("friends_leaderboard")]
        public Leaderboard FriendsLeaderboard { get; set; }
        [XmlElement("leaderboard_columns")]
        public LeaderboardColumns LeaderboardColumns { get; set; }
    }

    public class LeaderboardPlayerStatsResponse
    {
        [XmlElement("player_stats")]
        public LeaderboardPlayer PlayerStats { get; set; }
    }
}
