using System.Xml.Serialization;

namespace GameServer.Models.Response
{
    [XmlType(TypeName = "game_create")]    // TODO: Can we change this to XmlRoot?
    public class GameCreate
    {
        [XmlAttribute("id")]
        public int Id { get; set; }
    }
}
