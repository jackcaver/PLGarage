using System.Collections.Generic;
using System.Xml.Serialization;

namespace GameServer.Models.Response
{
    [XmlRoot("favorite_player")]
    public class FavoritePlayer 
    {
        [XmlAttribute("favorite_player_id")]
        public int FavoritePlayerId { get; set; }
        [XmlAttribute("hearted_by_me")]
        public int HeartedByMe { get; set; }
        [XmlAttribute("hearts")]
        public int Hearts { get; set; }
        [XmlAttribute("id")]
        public int Id { get; set; }
        [XmlAttribute("quote")]
        public string Quote { get; set; }
        [XmlAttribute("total_tracks")]
        public int TotalTracks { get; set; }
        [XmlAttribute("username")]
        public string Username { get; set; }
    }

    [XmlRoot("favorite_players")]
    public class FavoritePlayers
    {
        [XmlAttribute("total")]
        public int Total { get; set; }
        [XmlElement("favorite_player")]
        public List<FavoritePlayer> Players { get; set; }
    }
}