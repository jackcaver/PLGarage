using System.Collections.Generic;
using System.Xml.Serialization;

namespace GameServer.Models.Response
{
    public class GameListGame
    {
        [XmlAttribute]
        public string name { get; set; }
        [XmlAttribute]
        public string speed_class { get; set; }
        [XmlAttribute]
        public string game_type { get; set; }
        [XmlAttribute]
        public int id { get; set; }
        [XmlAttribute]
        public int game_state_id { get; set; }
        [XmlAttribute]
        public int host_player_id { get; set; }
        [XmlAttribute]
        public bool is_ranked { get; set; }
        [XmlAttribute]
        public int max_players { get; set; }
        [XmlAttribute]
        public int min_players { get; set; }
        [XmlAttribute]
        public int cur_players { get; set; }
        [XmlAttribute]
        public int number_laps { get; set; }
        [XmlAttribute]
        public int lobby_channel_id { get; set; }
        [XmlAttribute]
        public int track { get; set; }
        [XmlAttribute]
        public string host_player_ip_address { get; set; }
    }

    [XmlType(TypeName = "games")]
    public class GameList
    {
        [XmlAttribute]
        public int page { get; set; }
        [XmlAttribute]
        public int total_pages { get; set; }
        [XmlAttribute]
        public int total { get; set; }
        [XmlElement("game")]
        public List<GameListGame> Games { get; set; }
    }
}
