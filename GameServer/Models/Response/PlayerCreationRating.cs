using System.Xml.Serialization;

namespace GameServer.Models.Response
{
    public class player_creation_rating 
    {
        [XmlAttribute]
        public string comments { get; set; }
        [XmlAttribute]
        public bool rating { get; set; }
    }
}