using Newtonsoft.Json;

namespace GameServer.Models.ServerCommunication.Events;

public class PlayerSessionDestroyedEvent
{
    [JsonProperty("uuid")] public string SessionUuid { get; set; } = string.Empty;
}