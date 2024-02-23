using GameServer.Implementation.Player;
using Newtonsoft.Json;

namespace GameServer.Models.Config
{
    public class ModExplorerCity
    {
        public string Name { get; set; }
        public string Country { get; set; }
        public float Latitude { get; set; }
        public float Longitude { get; set; }
        [JsonIgnore]
        public float U => ModMile.GetU(Longitude);
        [JsonIgnore]
        public float V => ModMile.GetV(Latitude);
    }
}
