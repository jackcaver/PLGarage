using System.Xml.Serialization;

namespace GameServer.Models.Response
{
    [XmlRoot("content_update")]
    public class ContentUpdate
    {
        [XmlAttribute("available_date")]
        public string AvailableDate { get; set; }
        [XmlAttribute("content_update_type")]
        public string ContentUpdateType { get; set; }
        [XmlAttribute("content_url")]
        public string ContentUrl { get; set; }
        [XmlAttribute("created_at")]
        public string CreatedAt { get; set; }
        [XmlAttribute("data_md5")]
        public string DataMd5 { get; set; }
        [XmlAttribute("description")]
        public string Description { get; set; }
        [XmlAttribute("has_been_uploaded")]
        public bool HasBeenUploaded { get; set; }
        [XmlAttribute("id")]
        public int Id { get; set; }
        [XmlAttribute("name")]
        public string Name { get; set; }
        [XmlAttribute("platform")]
        public string Platform { get; set; }
        [XmlAttribute("updated_at")]
        public string UpdatedAt { get; set; }
        [XmlAttribute("uuid")]
        public string Uuid { get; set; }
        [XmlElement("data")]
        public string Data { get; set; }
    }
}
