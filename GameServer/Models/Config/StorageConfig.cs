using GameServer.Models.Config.Storage;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace GameServer.Models.Config
{
    public class StorageConfig
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public StorageType Type { get; set; } = StorageType.Local;
        
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public LocalStorageConfig Local { get; set; } = new();
        
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public S3StorageConfig S3 { get; set; } = null;
    }
}
