using System.Collections.Generic;
using System.Xml.Serialization;

namespace GameServer.Models.Response
{
    [XmlType(TypeName = "event")]    // TODO: Can we change this to XmlRoot?
    public class Event
    {
        [XmlAttribute("allusion_id")]
        public int AllusionId { get; set; }
        [XmlAttribute("allusion_type")]
        public string AllusionType { get; set; }
        [XmlAttribute("author_id")]
        public int AuthorId { get; set; }
        [XmlAttribute("author_username")]
        public string AuthorUsername { get; set; }
        [XmlAttribute("creator_id")]
        public int CreatorId { get; set; }
        [XmlAttribute("creator_username")]
        public string CreatorUsername { get; set; }
        [XmlAttribute("details")]
        public string Details { get; set; }
        [XmlAttribute("player_id")]
        public int PlayerId { get; set; }
        [XmlAttribute("seconds_ago")]
        public int SecondsAgo { get; set; }
        [XmlAttribute("tags")]
        public string Tags { get; set; }
        [XmlAttribute("timestamp")]
        public string Timestamp { get; set; }
        [XmlAttribute("topic")]
        public string Topic { get; set; }
        [XmlAttribute("type")]
        public string Type { get; set; }
        [XmlAttribute("username")]
        public string Username { get; set; }
        [XmlAttribute("image_url")]
        public string ImageUrl { get; set; }
        [XmlAttribute("image_md5")]
        public string ImageMd5 { get; set; }
        [XmlAttribute("subject")]
        public string Subject { get; set; }
    }

    [XmlType(TypeName = "activity")]    // TODO: Can we change this to XmlRoot?
    public class Activity
    {
        [XmlAttribute("player_creation_associated_item_ids")]
        public string PlayerCreationAssociatedItemIds { get; set; }
        [XmlAttribute("player_creation_description")]
        public string PlayerCreationDescription { get; set; }
        [XmlAttribute("player_creation_hearts")]
        public int PlayerCreationHearts { get; set; }
        [XmlAttribute("player_creation_id")]
        public int PlayerCreationId { get; set; }
        [XmlAttribute("player_creation_is_team_pick")]
        public bool PlayerCreationIsTeamPick { get; set; }
        [XmlAttribute("player_creation_level_mode")]
        public int PlayerCreationLevelMode { get; set; }
        [XmlAttribute("player_creation_name")]
        public string PlayerCreationName { get; set; }
        [XmlAttribute("player_creation_player_id")]
        public int PlayerCreationPlayerId { get; set; }
        [XmlAttribute("player_creation_races_started")]
        public int PlayerCreationRacesStarted { get; set; }
        [XmlAttribute("player_creation_rating_down")]
        public int PlayerCreationRatingDown { get; set; }
        [XmlAttribute("player_creation_rating_up")]
        public int PlayerCreationRatingUp { get; set; }
        [XmlAttribute("player_creation_username")]
        public string PlayerCreationUsername { get; set; }
        [XmlAttribute("player_hearts")]
        public int PlayerHearts { get; set; }
        [XmlAttribute("player_id")]
        public int PlayerId { get; set; }
        [XmlAttribute("player_username")]
        public string PlayerUsername { get; set; }
        [XmlAttribute("type")]
        public string Type { get; set; }
        [XmlElement("events")]
        public List<Event> Events { get; set; }
    }

    [XmlType(TypeName = "activities")]    // TODO: Can we change this to XmlRoot?
    public class Activities
    {
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
        [XmlElement("activity")]
        public List<Activity> ActivityList { get; set; }
    }
}