using System.Collections.Generic;
using System.Xml.Serialization;

namespace GameServer.Models.Response
{
    [XmlRoot("player_metric")]
    public class PlayerMetric 
    {
        [XmlAttribute("deviation")]
        public string Deviation { get; set; }
        [XmlAttribute("num_games")]
        public string NumGames { get; set; }
        [XmlAttribute("player_id")]
        public int PlayerId { get; set; }
        [XmlAttribute("points")]
        public int Points { get; set; }
        [XmlAttribute("volatility")]
        public string Volatility { get; set; }
    }

    [XmlRoot("player_metrics")]
    public class PlayerMetrics
    {
        [XmlAttribute("total")]
        public int Total { get; set; }
        [XmlElement("player_metric")]
        public List<PlayerMetric> Metrics { get; set; }
    }
}