using System.Collections.Generic;
using System.Xml.Serialization;

namespace GameServer.Models.Response
{
    [XmlType(TypeName = "comment")]
    public class Comment 
    {
        [XmlAttribute]
        public int id { get; set; }
        [XmlAttribute]
        public int player_id { get; set; }
        [XmlAttribute]
        public string username { get; set; }
        [XmlAttribute]
        public string body { get; set; }
        [XmlAttribute]
        public int rating_up { get; set; }
        [XmlAttribute]
        public bool rated_by_me { get; set; }
        [XmlAttribute]
        public string updated_at { get; set; }
    }

    [XmlType(TypeName = "track")]
    public class Track
    {
        [XmlAttribute]
        public bool ai { get; set; }
        [XmlAttribute]
        public string associated_item_ids { get; set; }
        [XmlAttribute]
        public bool auto_reset { get; set; }
        [XmlAttribute]
        public bool battle_friendly_fire { get; set; }
        [XmlAttribute]
        public int battle_kill_count { get; set; }
        [XmlAttribute]
        public int battle_time_limit { get; set; }
        [XmlAttribute]
        public float coolness { get; set; }
        [XmlAttribute]
        public string created_at { get; set; }
        [XmlAttribute]
        public string description { get; set; }
        [XmlAttribute]
        public string difficulty { get; set; }
        [XmlAttribute]
        public string dlc_keys { get; set; }
        [XmlAttribute]
        public int downloads { get; set; }
        [XmlAttribute]
        public int downloads_last_week { get; set; }
        [XmlAttribute]
        public int downloads_this_week { get; set; }
        [XmlAttribute]
        public string first_published { get; set; }
        [XmlAttribute]
        public string hearted_by_me { get; set; }
        [XmlAttribute]
        public int hearts { get; set; }
        [XmlAttribute]
        public int id { get; set; }
        [XmlAttribute]
        public bool is_remixable { get; set; }
        [XmlAttribute]
        public bool is_team_pick { get; set; }
        [XmlAttribute]
        public string last_published { get; set; }
        [XmlAttribute]
        public int level_mode { get; set; }
        [XmlAttribute]
        public float longest_drift { get; set; }
        [XmlAttribute]
        public float longest_hang_time { get; set; }
        [XmlAttribute]
        public string name { get; set; }
        [XmlAttribute]
        public int num_laps { get; set; }
        [XmlAttribute]
        public int num_racers { get; set; }
        [XmlAttribute]
        public int max_humans { get; set; }
        [XmlAttribute]
        public string platform { get; set; }
        [XmlAttribute]
        public string player_creation_type { get; set; }
        [XmlAttribute]
        public int player_id { get; set; }
        [XmlAttribute]
        public string queued_by_me { get; set; }
        [XmlAttribute]
        public string race_type { get; set; }
        [XmlAttribute]
        public int races_finished { get; set; }
        [XmlAttribute]
        public int races_started { get; set; }
        [XmlAttribute]
        public int races_started_this_month { get; set; }
        [XmlAttribute]
        public int races_started_this_week { get; set; }
        [XmlAttribute]
        public int races_won { get; set; }
        [XmlAttribute]
        public int rank { get; set; }
        [XmlAttribute]
        public int rating_down { get; set; }
        [XmlAttribute]
        public int rating_up { get; set; }
        [XmlAttribute]
        public string reviewed_by_me { get; set; }
        [XmlAttribute]
        public int scoreboard_mode { get; set; }
        [XmlAttribute]
        public string speed { get; set; }
        [XmlAttribute]
        public string tags { get; set; }
        [XmlAttribute]
        public int track_theme { get; set; }
        [XmlAttribute]
        public int unique_racer_count { get; set; }
        [XmlAttribute]
        public string updated_at { get; set; }
        [XmlAttribute]
        public string user_tags { get; set; }
        [XmlAttribute]
        public string username { get; set; }
        [XmlAttribute]
        public int version { get; set; }
        [XmlAttribute]
        public int views { get; set; }
        [XmlAttribute]
        public int views_last_week { get; set; }
        [XmlAttribute]
        public int views_this_week { get; set; }
        [XmlAttribute]
        public int votes { get; set; }
        [XmlAttribute]
        public string weapon_set { get; set; }
        public List<Comment> comments { get; set; }
        [XmlElement("reviews")]
        public List<Reviews> reviews { get; set; }
        [XmlElement("activities")]
        public List<Activities> activities { get; set; }
        [XmlElement("photos")]
        public List<Photos> photos { get; set; }
        [XmlElement("leaderboard")]
        public List<SubLeaderboard> leaderboard { get; set; }
    }
}