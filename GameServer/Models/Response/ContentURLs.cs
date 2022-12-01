using System.Collections.Generic;
using System.Xml.Serialization;

namespace GameServer.Models.Response
{
    public class ContentURL
    {
        [XmlAttribute]
        public string formats { get; set; }
        [XmlAttribute]
        public string name { get; set; }
        [XmlText]
        public string url { get; set; }
    }
    public class ContentURLs
    {
        [XmlAttribute]
        public int total { get; set; }
        [XmlAttribute]
        public string server_uuid { get; set; }
        [XmlElement("content_url")]
        public List<ContentURL> ContentURLList { get; set; }
    }

    public class MagicMoment
    {
        [XmlAttribute]
        public bool scea { get; set; }
        [XmlAttribute]
        public bool scee { get; set; }
        [XmlAttribute]
        public bool sceasia { get; set; }
        [XmlAttribute]
        public bool scej { get; set; }
    }

    public class ContentURLsResponse
    {
        public ContentURLs content_urls { get; set; }
        public MagicMoment magic_moment { get; set; }
    }
}