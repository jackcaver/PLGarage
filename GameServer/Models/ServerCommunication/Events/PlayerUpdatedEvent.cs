using Newtonsoft.Json;

namespace GameServer.Models.ServerCommunication.Events;

public class PlayerUpdatedEvent
{
    [JsonProperty("id")] public int PlayerConnectId { get; set; }
    [JsonProperty("char")] public int CharacterId { get; set; }
    [JsonProperty("kart")] public int KartId { get; set; }
}