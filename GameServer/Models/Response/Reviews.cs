using System.Collections.Generic;
using System.Xml.Serialization;

namespace GameServer.Models.Response
{
    [XmlType(TypeName = "review")]    // TODO: Can we change this to XmlRoot?
    public class Review 
    {
        [XmlAttribute("content")]
        public string Content { get; set; }
        [XmlAttribute("id")]
        public int Id { get; set; }
        [XmlAttribute("mine")]
        public string Mine { get; set; }
        [XmlAttribute("player_creation_id")]
        public int PlayerCreationId { get; set; }
        [XmlAttribute("player_creation_name")]
        public string PlayerCreationName { get; set; }
        [XmlAttribute("player_creation_username")]
        public string PlayerCreationUsername { get; set; }
        [XmlAttribute("player_creation_associated_item_ids")]
        public string PlayerCreationAssociatedItemIds { get; set; }
        [XmlAttribute("player_creation_level_mode")]
        public string PlayerCreationLevelMode { get; set; }
        [XmlAttribute("player_creation_is_team_pick")]
        public string PlayerCreationIsTeamPick { get; set; }
        [XmlAttribute("player_id")]
        public int PlayerId { get; set; }
        [XmlAttribute("rated_by_me")]
        public string RatedByMe { get; set; }
        [XmlAttribute("rating_down")]
        public string RatingDown { get; set; }
        [XmlAttribute("rating_up")]
        public string RatingUp { get; set; }
        [XmlAttribute("username")]
        public string Username { get; set; }
        [XmlAttribute("tags")]
        public string Tags { get; set; }
        [XmlAttribute("updated_at")]
        public string UpdatedAt { get; set; }
    }

    [XmlType(TypeName = "reviews")]    // TODO: Can we change this to XmlRoot?
    public class Reviews
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
        [XmlElement("review")]
        public List<Review> ReviewList { get; set; }
    }
}