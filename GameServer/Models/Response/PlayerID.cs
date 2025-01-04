using System.Xml.Serialization;

namespace GameServer.Models.Response
{
    public class PlayerIDResponse
    {
        [XmlElement("player_id")]
        public int PlayerId { get; set; }
    }
}