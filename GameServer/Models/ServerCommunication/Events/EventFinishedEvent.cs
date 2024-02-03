using System.Collections.Generic;
using Newtonsoft.Json;

namespace GameServer.Models.ServerCommunication.Events;

public class EventFinishedEvent
{
    [JsonProperty("id")] public int TrackId { get; set; }
    [JsonProperty("stats")] public List<PlayerEventStats> Stats { get; set; } = new();
}