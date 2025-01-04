using System.Xml.Serialization;

namespace GameServer.Models.Response
{
    [XmlType(TypeName = "preference")]    // TODO: Can we change this to XmlRoot?
    public class Preference
    {
        [XmlAttribute("domain")]
        public string Domain { get; set; }
        [XmlAttribute("ip_address")]
        public string IpAddress { get; set; }
        [XmlAttribute("language_code")]
        public string LanguageCode { get; set; }
        [XmlAttribute("region_code")]
        public string RegionCode { get; set; }
        [XmlAttribute("timezone")]
        public string Timezone { get; set; }
    }
}