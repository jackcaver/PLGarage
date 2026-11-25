using GameServer.Models.PlayerData.PlayerCreations;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace GameServer.Models.Moderation
{
    public class MinimalCreationInfo
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public PlayerCreationType Type { get; set; }
        public int PlayerID { get; set; }
        public string Username { get; set; }
        public int ParentPlayerID { get; set; }
        public string ParentUsername { get; set; }
        public int OriginalPlayerID { get; set; }
        public string OriginalUsername { get; set; }
        public int ParentCreationID { get; set; }
        public string ParentCreationName { get; set; }
        public ModerationStatus ModerationStatus { get; set; }
        public bool IsMNR { get; set; }
    }
}
