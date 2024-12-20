using GameServer.Models.PlayerData;
using Newtonsoft.Json;

namespace GameServer.Models.ServerCommunication.Events;

public class PlayerSessionCreatedEvent
{
    [JsonProperty("uuid")] public string SessionUuid { get; set; } = string.Empty;
    [JsonProperty("id")] public int PlayerConnectId { get; set; }
    [JsonProperty("user")] public string Username { get; set; } = string.Empty;
    [JsonProperty("issuer")] public int Issuer { get; set; }
    [JsonProperty("platform")] public Platform SessionPlatform { get; set; }
}