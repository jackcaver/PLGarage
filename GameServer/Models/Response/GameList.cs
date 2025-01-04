using System.Collections.Generic;
using System.Xml.Serialization;

namespace GameServer.Models.Response
{
    public class GameListGame
    {
        [XmlAttribute("name")]
        public string Name { get; set; }
        [XmlAttribute("speed_class")]
        public string SpeedClass { get; set; }
        [XmlAttribute("game_type")]
        public string GameType { get; set; }
        [XmlAttribute("id")]
        public int Id { get; set; }
        [XmlAttribute("game_state_id")]
        public int GameStateId { get; set; }
        [XmlAttribute("host_player_id")]
        public int HostPlayerId { get; set; }
        [XmlAttribute("is_ranked")]
        public bool IsRanked { get; set; }
        [XmlAttribute("max_players")]
        public int MaxPlayers { get; set; }
        [XmlAttribute("min_players")]
        public int MinPlayers { get; set; }
        [XmlAttribute("cur_players")]
        public int CurPlayers { get; set; }
        [XmlAttribute("number_laps")]
        public int number_laps { get; set; }
        [XmlAttribute("lobby_channel_id")]
        public int LobbyChannelId { get; set; }
        [XmlAttribute("track")]
        public int Track { get; set; }
        [XmlAttribute("host_player_ip_address")]
        public string HostPlayerIpAddress { get; set; } // TODO: Could this possibly expose users IP addresses? (this might not be used but best to double check)
    }

    [XmlType(TypeName = "games")]    // TODO: Can we change this to XmlRoot?
    public class GameList
    {
        [XmlAttribute("page")]
        public int Page { get; set; }
        [XmlAttribute("total_pages")]
        public int TotalPages { get; set; }
        [XmlAttribute("total")]
        public int Total { get; set; }
        [XmlElement("game")]
        public List<GameListGame> Games { get; set; }
    }
}
