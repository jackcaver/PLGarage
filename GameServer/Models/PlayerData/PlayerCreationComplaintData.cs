using GameServer.Models.PlayerData.PlayerCreations;
using GameServer.Models.Request;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameServer.Models.PlayerData
{
    public class PlayerCreationComplaintData
    {
        [Key]
        [JsonIgnore]
        public int Id { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public PlayerComplaintReason Reason { get; set; }
        public string Comments { get; set; }

        [JsonIgnore]
        public User User { get; set; }
        [JsonIgnore]
        public User Player { get; set; }
        [JsonIgnore]
        public PlayerCreationData PlayerCreation { get; set; }
    }
}
