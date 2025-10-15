using System.Collections.Generic;
using Newtonsoft.Json;

namespace GameServer.Models.ServerCommunication.Events;

public class EventStartedEvent
{
    [JsonProperty("id")] public int TrackId { get; set; }
    [JsonProperty("players")] public List<int> PlayerIds { get; set; } = [];
    [JsonProperty("isMNR")] public bool IsMNR { get; set; }
}