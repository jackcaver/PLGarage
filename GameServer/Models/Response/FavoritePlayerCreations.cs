using System.Collections.Generic;
using System.Xml.Serialization;

namespace GameServer.Models.Response
{
    [XmlRoot("favorite_player_creation")]
    public class FavoritePlayerCreation 
    {
        [XmlAttribute("player_creation_id")]
        public int PlayerCreationId { get; set; }
        [XmlAttribute("player_creation_name")]
        public string PlayerCreationName { get; set; }
    }

    [XmlRoot("favorite_player_creations")]
    public class FavoritePlayerCreations
    {
        [XmlAttribute("total")]
        public int Total { get; set; }
        public List<FavoritePlayerCreation> PlayerCreations { get; set; }
    }
}