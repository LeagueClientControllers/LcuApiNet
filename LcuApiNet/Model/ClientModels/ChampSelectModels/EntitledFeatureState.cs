using Newtonsoft.Json;

namespace LcuApiNet.Model.ClientModels.ChampSelectModels;

public class EntitledFeatureState
{
    [JsonProperty("additionalRerolls")]
    public long AdditionalRerolls { get; set; }

    [JsonProperty("unlockedSkinIds")]
    public object[] UnlockedSkinIds { get; set; }
}