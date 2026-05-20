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
        public string points { get; set; }
        [XmlAttribute]
        public string volatility { get; set; }
        //MNR
        [XmlAttribute]
        public string created_at { get; set; }
        [XmlAttribute]
        public string data { get; set; }
        [XmlAttribute]
        public string id { get; set; }
        [XmlAttribute]
        public string player_metric_type { get; set; }
        [XmlAttribute]
        public string updated_at { get; set; }
        [XmlAttribute]
        public string username { get; set; }
    }

    public class player_metrics
    {
        [XmlAttribute]
        public int total { get; set; }
        [XmlElement("player_metric")]
        public List<player_metric> Metrics { get; set; }
    }
}