using Newtonsoft.Json;

namespace GameServer.Models.ServerCommunication.Events;

public class PlayerQuitEvent
{
    [JsonProperty("id")] public int PlayerConnectId { get; set; }
    [JsonProperty("disconnect")] public bool Disconnected { get; set; }
}