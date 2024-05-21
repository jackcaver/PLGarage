using System.Xml.Serialization;

namespace GameServer.Models.Response
{
    [XmlType(TypeName = "game_create")]
    public class GameCreate
    {
        [XmlAttribute]
        public int id { get; set; }
    }
}
