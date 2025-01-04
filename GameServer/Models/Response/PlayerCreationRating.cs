using System.Collections.Generic;
using System.Xml.Serialization;

namespace GameServer.Models.Response
{
    [XmlRoot("player_creation_rating")]
    public class PlayerCreationRating
    {
        [XmlAttribute("comments")]
        public string Comments { get; set; }
        [XmlAttribute("rating")]
        public string Rating { get; set; }
        //MNR
        [XmlAttribute("player_id")]
        public string PlayerId { get; set; }
        [XmlAttribute("username")]
        public string Username { get; set; }
    }

    [XmlRoot("player_creation_ratings")]
    public class PlayerCreationRatings
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
        [XmlElement("player_creation_rating")]
        public List<PlayerCreationRating> PlayerCreationRatingList { get; set; }
    }
}