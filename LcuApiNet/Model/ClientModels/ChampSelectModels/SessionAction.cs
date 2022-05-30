using Ardalis.SmartEnum.JsonNet;
using LcuApiNet.Model.Enums;
using LcuApiNet.Utilities;
using Newtonsoft.Json;

namespace LcuApiNet.Model.ClientModels.ChampSelectModels;

public class SessionAction
{
    [JsonProperty("actorCellId")]
    public int ActorCellId { get; set; }

    [JsonProperty("championId")]
    public int ChampionId { get; set; }

    [JsonProperty("completed")]
    public bool Completed { get; set; }

    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("isAllyAction")]
    public bool IsAllyAction { get; set; }

    [JsonProperty("isInProgress")]
    public bool IsInProgress { get; set; }

    [JsonProperty("pickTurn")]
    public int PickTurn { get; set; }

    [JsonProperty("type")]
    [JsonConverter(typeof(SmartEnumNameConverter<ActionType, int>))]
    public ActionType Type { get; set; }
}
