using Newtonsoft.Json;

namespace GameServer.Models.ServerCommunication.Events;

public class UpdatePlayerCountEvent
{
    [JsonProperty("count")] public int PlayerCount { get; set; }
}