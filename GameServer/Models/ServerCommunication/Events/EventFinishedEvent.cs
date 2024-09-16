using System.Collections.Generic;
using GameServer.Models.GameBrowser;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace GameServer.Models.ServerCommunication.Events;

public class EventFinishedEvent
{
    [JsonProperty("id")] public int TrackId { get; set; }
    [JsonProperty("stats")] public List<PlayerEventStats> Stats { get; set; } = [];
    [JsonProperty("isMNR")] public bool IsMNR { get; set; }
    [JsonProperty("gameType")] [JsonConverter(typeof(StringEnumConverter))] public GameType GameType { get; set; }
}