using Ardalis.SmartEnum.JsonNet;
using LcuApiNet.Model.Enums;
using Newtonsoft.Json;

namespace LcuApiNet.Model.ClientModels.ChampSelectModels;

public class TeamMember
{
    [JsonProperty("assignedPosition")]
    public string AssignedPosition { get; set; }

    [JsonProperty("cellId")]
    public int CellId { get; set; }

    [JsonProperty("championId")]
    public int ChampionId { get; set; }

    [JsonProperty("championPickIntent")]
    public int ChampionPickIntent { get; set; }

    [JsonProperty("entitledFeatureType")]
    public string EntitledFeatureType { get; set; }

    [JsonProperty("selectedSkinId")]
    public int SelectedSkinId { get; set; }

    [JsonProperty("spell1Id")]
    public int Spell1Id { get; set; }

    [JsonProperty("spell2Id")]
    public int Spell2Id { get; set; }

    [JsonProperty("summonerId")]
    public long SummonerId { get; set; }

    [JsonProperty("team")]
    public bool TeamSide { get; set; }

    [JsonProperty("wardSkinId")]
    public int WardSkinId { get; set; }
}