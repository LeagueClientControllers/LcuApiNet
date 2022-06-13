using System.ComponentModel;
using System.Runtime.CompilerServices;
using Ardalis.SmartEnum.JsonNet;
using LcuApiNet.Annotations;
using LcuApiNet.Model.Enums;
using LcuApiNet.Utilities;
using Newtonsoft.Json;

namespace LcuApiNet.Model.ClientModels.ChampSelectModels;

public class SessionAction : INotifyPropertyChanged
{
    [JsonProperty("actorCellId")]
    public int ActorCellId { get; set; }

    private int _championId;

    [JsonProperty("championId")]
    public int ChampionId
    {
        get => _championId;
        set
        {
            if (_championId == value) return;
            _championId = value;
            OnPropertyChanged(nameof(ChampionId));
        }
    }

    private bool _completed;

    [JsonProperty("completed")]
    public bool Completed
    {
        get => _completed;
        set
        {
            if (_completed == value) return;
            _completed = value;
            OnPropertyChanged(nameof(Completed));
        }
    }

    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("isAllyAction")]
    public bool IsAllyAction  { get; set; }

    private bool _isInProgress;

    [JsonProperty("isInProgress")]
    public bool IsInProgress
    {
        get => _isInProgress;
        set
        {
            if (_isInProgress == value) return;
            _isInProgress = value;
            OnPropertyChanged(nameof(IsInProgress));
        }
    }

    [JsonProperty("pickTurn")]
    public int PickTurn { get; set; }

    [JsonProperty("type")]
    [JsonConverter(typeof(SmartEnumNameConverter<ActionType, int>))]
    public ActionType Type { get; set; }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    
    public void ApplyChanges(SessionAction other)
    {
        this.Id = other.Id;
        this.ChampionId = other.ChampionId;
        this.ActorCellId = other.ActorCellId;
        this.Completed = other.Completed;
        this.Type = other.Type;
        this.IsAllyAction = other.IsAllyAction;
        this.IsInProgress = other.IsInProgress;    
    }
}
