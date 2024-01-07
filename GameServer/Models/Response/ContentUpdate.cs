using System.Xml.Serialization;

namespace GameServer.Models.Response
{
    public class content_update
    {
        [XmlAttribute]
        public string available_date { get; set; }
        [XmlAttribute]
        public string content_update_type { get; set; }
        [XmlAttribute]
        public string content_url { get; set; }
        [XmlAttribute]
        public string created_at { get; set; }
        [XmlAttribute]
        public string data_md5 { get; set; }
        [XmlAttribute]
        public string description { get; set; }
        [XmlAttribute]
        public bool has_been_uploaded { get; set; }
        [XmlAttribute]
        public int id { get; set; }
        [XmlAttribute]
        public string name { get; set; }
        [XmlAttribute]
        public string platform { get; set; }
        [XmlAttribute]
        public string updated_at { get; set; }
        [XmlAttribute]
        public string uuid { get; set; }
        public string data { get; set; }
    }
}
