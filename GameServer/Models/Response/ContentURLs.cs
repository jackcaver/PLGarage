using System.Collections.Generic;
using System.Xml.Serialization;

namespace GameServer.Models.Response
{
    public class ContentURL
    {
        [XmlAttribute("formats")]
        public string Formats { get; set; }
        [XmlAttribute("name")]
        public string Name { get; set; }
        [XmlText]
        public string Url { get; set; }
    }
    public class ContentURLs
    {
        [XmlAttribute("total")]
        public int Total { get; set; }
        [XmlAttribute("server_uuid")]
        public string ServerUuid { get; set; }
        [XmlElement("content_url")]
        public List<ContentURL> ContentURLList { get; set; }
    }

    public class MagicMoment
    {
        [XmlAttribute("scea")]
        public bool Scea { get; set; }
        [XmlAttribute("scee")]
        public bool Scee { get; set; }
        [XmlAttribute("sceasia")]
        public bool Sceasia { get; set; }
        [XmlAttribute("scej")]
        public bool Scej { get; set; }
    }

    public class ContentURLsResponse
    {
        [XmlElement("content_urls")]
        public ContentURLs ContentUrls { get; set; }
        [XmlElement("magic_moment")]
        public MagicMoment MagicMoment { get; set; }
    }
}