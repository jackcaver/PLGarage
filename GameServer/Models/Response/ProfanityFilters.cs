using System.Collections.Generic;
using System.Xml.Serialization;

namespace GameServer.Models.Response
{
    public class profanity_filter 
    {
        
        [XmlAttribute]
        public string pattern { get; set; }
        [XmlAttribute]
        public string replace { get; set; }
    }

    public class profanity_filters
    {
        [XmlElement("profanity_filter")]
        public List<profanity_filter> ProfanityFilterList { get; set; }
    }
}