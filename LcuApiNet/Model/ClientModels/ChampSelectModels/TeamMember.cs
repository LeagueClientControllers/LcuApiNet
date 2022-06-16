using System.ComponentModel;
using System.Runtime.CompilerServices;
using Ardalis.SmartEnum.JsonNet;
using LcuApiNet.Annotations;
using LcuApiNet.Model.Enums;
using Newtonsoft.Json;

namespace LcuApiNet.Model.ClientModels.ChampSelectModels;

public class TeamMember : INotifyPropertyChanged
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

    private int _selectedSkinId;

    [JsonProperty("selectedSkinId")]
    public int SelectedSkinId
    {
        get => _selectedSkinId;
        set
        {
            if (_selectedSkinId == value) return;
            _selectedSkinId = value;
            OnPropertyChanged(nameof(SelectedSkinId));
        }
    }

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

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public void ApplyChanges(TeamMember other)
    {
        this.SelectedSkinId = other.SelectedSkinId;
    }
}