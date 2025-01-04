using System.Xml.Serialization;

namespace GameServer.Models.Response
{
    [XmlType(TypeName = "game")]    // TODO: Can we change this to XmlRoot?
    public class GameResponse
    {
        [XmlAttribute("id")]
        public int Id { get; set; }
        [XmlAttribute("game_player_id")]
        public int GamePlayerId { get; set; }
        [XmlAttribute("game_player_stats_id")]
        public int GamePlayerStatsId { get; set; }
    }
}