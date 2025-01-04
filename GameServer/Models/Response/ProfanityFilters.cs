using System.Collections.Generic;
using System.Xml.Serialization;

namespace GameServer.Models.Response
{
    [XmlRoot("profanity_filter")]
    public class ProfanityFilter 
    {
        
        [XmlAttribute("pattern")]
        public string Pattern { get; set; }
        [XmlAttribute("replace")]
        public string Replace { get; set; }
    }

    [XmlRoot("profanity_filters")]
    public class ProfanityFilters
    {
        [XmlElement("profanity_filter")]
        public List<ProfanityFilter> ProfanityFilterList { get; set; }
    }
}