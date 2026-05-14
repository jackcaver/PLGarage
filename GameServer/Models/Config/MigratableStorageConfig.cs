using Newtonsoft.Json;

namespace GameServer.Models.Config
{
    public class MigratableStorageConfig : StorageConfig
    {
        public bool Migrate { get; set; } = false;
        
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public StorageConfig MigrateFrom { get; set; } = null;
    }
}
