using System.Collections.Generic;
using System.Xml.Serialization;

namespace GameServer.Models.Response
{
    [XmlType(TypeName = "photo")]    // TODO: Can we change this to XmlRoot?
    public class Photo 
    {        
        [XmlAttribute("associated_usernames")]
        public string AssociatedUsernames { get; set; }
        [XmlAttribute("id")]
        public int Id { get; set; }
        [XmlAttribute("track_id")]
        public int TrackId { get; set; }
        [XmlAttribute("username")]
        public string Username { get; set; }
    }

    [XmlType(TypeName = "photos")]    // TODO: Can we change this to XmlRoot?
    public class Photos
    {
        [XmlAttribute("current_page")]
        public int CurrentPage { get; set; }
        [XmlAttribute("row_end")]
        public int RowEnd { get; set; }
        [XmlAttribute("row_start")]
        public int RowStart { get; set; }
        [XmlAttribute("total")]
        public int Total { get; set; }
        [XmlAttribute("total_pages")]
        public int TotalPages { get; set; }
        [XmlElement("photo")]
        public List<Photo> PhotoList { get; set; }
    }
}