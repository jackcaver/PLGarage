using System.Collections.Generic;
using System.Xml.Serialization;

namespace GameServer.Models.Response
{
    public class player_metric 
    {
        [XmlAttribute]
        public string deviation { get; set; }
        [XmlAttribute]
        public string num_games { get; set; }
        [XmlAttribute]
        public int player_id { get; set; }
        [XmlAttribute]
        public int points { get; set; }
        [XmlAttribute]
        public string volatility { get; set; }
    }

    public class player_metrics
    {
        [XmlAttribute]
        public int total { get; set; }
        [XmlElement("player_metric")]
        public List<player_metric> Metrics { get; set; }
    }
}