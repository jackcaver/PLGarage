using System.Collections.Generic;
using System.Xml.Serialization;

namespace GameServer.Models.Response
{
    [XmlRoot("player_comment")]
    public class PlayerComment 
    {
        [XmlAttribute("body")]
        public string Body { get; set; }
        [XmlAttribute("created_at")]
        public string CreatedAt { get; set; }
        [XmlAttribute("id")]
        public int Id { get; set; }
        [XmlAttribute("platform")]
        public string Platform { get; set; }
        [XmlAttribute("author_id")]
        public int AuthorId { get; set; }
        [XmlAttribute("player_id")]
        public int PlayerId { get; set; }
        [XmlAttribute("updated_at")]
        public string UpdatedAt { get; set; }
        [XmlAttribute("author_username")]
        public string AuthorUsername { get; set; }
        [XmlAttribute("username")]
        public string Username { get; set; }
        [XmlAttribute("rating_up")]
        public int RatingUp { get; set; }
        [XmlAttribute("rating_down")]
        public int RatingDown { get; set; }
        [XmlAttribute("rated_by_me")]
        public bool RatedByMe { get; set; }
    }

    [XmlRoot("player_comments")]
    public class PlayerComments
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
        [XmlElement("player_comment")]
        public List<PlayerComment> PlayerCommentList { get; set; }
    }
}