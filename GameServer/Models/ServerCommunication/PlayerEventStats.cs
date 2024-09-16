using Newtonsoft.Json;

namespace GameServer.Models.ServerCommunication;

public class PlayerEventStats
{
    [JsonProperty("id")] public int PlayerConnectId { get; set; }
    [JsonProperty("hang")] public float BestHangTime { get; set; }
    [JsonProperty("drift")] public float BestDrift { get; set; }
    [JsonProperty("finished")] public bool Finished { get; set; }
    [JsonProperty("rank")] public int Rank { get; set; }
    [JsonProperty("playgroupSize")] public int PlaygroupSize { get; set; }
    [JsonProperty("finish")] public float FinishTime { get; set; }
    [JsonProperty("lap")] public float BestLapTime { get; set; }
    [JsonProperty("score")] public float Points { get; set; }
}