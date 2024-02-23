using GameServer.Implementation.Player;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace GameServer.Models.Config
{
    public class PointOfInterest
    {
        public int CityId { get; set; }
        public string Name { get; set; }
        public float Latitude { get; set; }
        public float Longitude { get; set; }
        public float Radius { get; set; }
        [JsonIgnore]
        public float U => ModMile.GetU(Longitude);
        [JsonIgnore]
        public float V => ModMile.GetV(Latitude);
        public List<PointOfInterestAward> Awards { get; set; }
    }
}
