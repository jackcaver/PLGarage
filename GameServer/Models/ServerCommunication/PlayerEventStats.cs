using Newtonsoft.Json;

namespace GameServer.Models.ServerCommunication;

public class PlayerEventStats
{
    [JsonProperty("id")] public int PlayerConnectId { get; set; }
    [JsonProperty("hang")] public float BestHangTime { get; set; }
    [JsonProperty("drift")] public float BestDrift { get; set; }
    [JsonProperty("finished")] public bool Finished { get; set; }
    [JsonProperty("rank")] public int Rank { get; set; }
}