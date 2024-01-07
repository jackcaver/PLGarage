using System.Collections.Generic;
using System.Xml.Serialization;

namespace GameServer.Models.Response
{
    public class LeaderboardPlayer
    {
        [XmlAttribute]
        public float best_lap_time { get; set; }
        [XmlAttribute]
        public int character_idx { get; set; }
        [XmlAttribute]
        public string created_at { get; set; }
        [XmlAttribute]
        public float creator_points { get; set; }
        [XmlAttribute]
        public float deviation { get; set; }
        [XmlAttribute]
        public float experience_points { get; set; }
        [XmlAttribute]
        public string game_end { get; set; }
        [XmlAttribute]
        public string game_start { get; set; }
        [XmlAttribute]
        public string game_type { get; set; }
        [XmlAttribute]
        public string ghost_car_data_md5 { get; set; }
        [XmlAttribute]
        public int id { get; set; }
        [XmlAttribute]
        public int kart_idx { get; set; }
        [XmlAttribute]
        public float longest_drift { get; set; }
        [XmlAttribute]
        public float longest_hang_time { get; set; }
        [XmlAttribute]
        public int longest_win_streak { get; set; }
        [XmlAttribute]
        public int online_disconnected { get; set; }
        [XmlAttribute]
        public int online_finished { get; set; }
        [XmlAttribute]
        public int online_forfeit { get; set; }
        [XmlAttribute]
        public int online_quits { get; set; }
        [XmlAttribute]
        public int online_races { get; set; }
        [XmlAttribute]
        public int online_wins { get; set; }
        [XmlAttribute]
        public string platform { get; set; }
        [XmlAttribute]
        public int player_id { get; set; }
        [XmlAttribute]
        public float points { get; set; }
        [XmlAttribute]
        public int rank { get; set; }
        [XmlAttribute]
        public int skill_level_id { get; set; }
        [XmlAttribute]
        public string skill_level_name { get; set; }
        [XmlAttribute]
        public int track_idx { get; set; }
        [XmlAttribute]
        public string type { get; set; }
        [XmlAttribute]
        public string updated_at { get; set; }
        [XmlAttribute]
        public string username { get; set; }
        [XmlAttribute]
        public float volatility { get; set; }
        [XmlAttribute]
        public int win_streak { get; set; }
    }

    public class Leaderboard
    {
        [XmlAttribute]
        public string game_type { get; set; }
        [XmlAttribute]
        public int page { get; set; }
        [XmlAttribute]
        public int row_end { get; set; }
        [XmlAttribute]
        public int row_start { get; set; }
        [XmlAttribute]
        public int total { get; set; }
        [XmlAttribute]
        public int total_pages { get; set; }
        [XmlAttribute]
        public string type { get; set; }
        [XmlElement("player")]
        public List<LeaderboardPlayer> LeaderboardPlayersList { get; set; }
    }

    public class LeaderboardColumn
    {
        [XmlAttribute]
        public string display_name { get; set; }
        [XmlAttribute]
        public string name { get; set; }
    }

    public class LeaderboardColumns
    {
        [XmlElement("column")]
        public List<LeaderboardColumn> Columns { get; set; }
    }

    public class LeaderboardViewResponse
    {
        public LeaderboardPlayer my_stats { get; set; }
        public Leaderboard leaderboard { get; set; }
        public LeaderboardColumns leaderboard_columns { get; set; }
    }

    public class LeaderboardFriendsViewResponse
    {
        public LeaderboardPlayer my_stats { get; set; }
        public Leaderboard friends_leaderboard { get; set; }
        public LeaderboardColumns leaderboard_columns { get; set; }
    }

    public class LeaderboardPlayerStatsResponse
    {
        public LeaderboardPlayer player_stats { get; set; }
    }
}
