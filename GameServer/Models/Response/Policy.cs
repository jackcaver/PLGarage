using System.Xml.Serialization;

namespace GameServer.Models.Response
{
    [XmlType(TypeName = "policy")]
    public class PolicyResponse
    {
        [XmlAttribute]
        public int id { get; set; }
        [XmlAttribute]
        public bool is_accepted { get; set; }
        [XmlAttribute]
        public string name { get; set; }

        [XmlText]
        public string text { get; set; }
    }
}