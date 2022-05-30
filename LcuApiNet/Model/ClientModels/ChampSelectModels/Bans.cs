using Newtonsoft.Json;

namespace LcuApiNet.Model.ClientModels.ChampSelectModels;

public class Bans
{
    [JsonProperty("myTeamBans")]
    public int[] MyTeamBans { get; set; }

    [JsonProperty("numBans")]
    public int NumBans { get; set; }

    [JsonProperty("theirTeamBans")]
    public int[] TheirTeamBans { get; set; }
}