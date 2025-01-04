using System.Collections.Generic;
using System.Xml.Serialization;

namespace GameServer.Models.Response
{
    [XmlRoot("player_creation_comment")]
    public class PlayerCreationComment
    {
        [XmlAttribute("body")]
        public string Body { get; set; }
        [XmlAttribute("created_at")]
        public string CreatedAt { get; set; }
        [XmlAttribute("id")]
        public int Id { get; set; }
        [XmlAttribute("platform")]
        public string Platform { get; set; }
        [XmlAttribute("player_creation_id")]
        public int PlayerCreationId { get; set; }
        [XmlAttribute("player_id")]
        public int PlayerId { get; set; }
        [XmlAttribute("updated_at")]
        public string UpdatedAt { get; set; }
        [XmlAttribute("username")]
        public string Username { get; set; }
        [XmlAttribute("rating_up")]
        public int RatingUp { get; set; }
        [XmlAttribute("rating_down")]
        public int RatingDown { get; set; }
        [XmlAttribute("rated_by_me")]
        public bool RatedByMe { get; set; }
    }

    [XmlRoot("player_creation_comments")]
    public class PlayerCreationComments
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
        [XmlElement("player_creation_comment")]
        public List<PlayerCreationComments> PlayerCreationCommentList { get; set; }
    }
}