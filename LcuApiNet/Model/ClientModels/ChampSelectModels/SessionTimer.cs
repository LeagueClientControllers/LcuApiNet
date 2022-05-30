using Newtonsoft.Json;

namespace LcuApiNet.Model.ClientModels.ChampSelectModels;

public class SessionTimer
{
    [JsonProperty("adjustedTimeLeftInPhase")]
    public long AdjustedTimeLeftInPhase { get; set; }

    [JsonProperty("internalNowInEpochMs")]
    public long InternalNowInEpochMs { get; set; }

    [JsonProperty("isInfinite")]
    public bool IsInfinite { get; set; }

    [JsonProperty("phase")]
    public string Phase { get; set; }

    [JsonProperty("totalTimeInPhase")]
    public long TotalTimeInPhase { get; set; }
}