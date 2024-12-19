using GameServer.Models.Request;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameServer.Models.PlayerData
{
    public class PlayerComplaintData
    {
        [JsonIgnore]
        public int Id { get; set; }
        public int UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        [JsonIgnore]
        public User User { get; set; }

        public int PlayerId { get; set; }

        [ForeignKey(nameof(PlayerId))]
        [JsonIgnore]
        public User Player { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public PlayerComplaintReason Reason { get; set; }
        public string Comments { get; set; }
    }
}
