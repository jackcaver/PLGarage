using System.Xml.Serialization;

namespace GameServer.Models.Response
{
    [XmlType(TypeName = "policy")]    // TODO: Can we change this to XmlRoot?
    public class PolicyResponse
    {
        [XmlAttribute("id")]
        public int Id { get; set; }
        [XmlAttribute("is_accepted")]
        public bool IsAccepted { get; set; }
        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlText]
        public string Text { get; set; }
    }
}