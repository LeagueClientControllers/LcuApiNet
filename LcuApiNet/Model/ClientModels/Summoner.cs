using System.ComponentModel;
using System.Runtime.CompilerServices;

using LcuApiNet.Annotations;

using Newtonsoft.Json;

namespace LcuApiNet.Model.ClientModels;

public class Summoner : INotifyPropertyChanged
{
    private string _name = default!;
    private string _gameTag = default!;
    private long _iconId;
    
    [JsonProperty("gameName")]
    public string Name { 
        get => _name;
        set {
            if (_name != value) {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }
    }
    
    [JsonProperty("gameTag")]
    public string GameTag {
        get => _gameTag;
        set {
            if (_gameTag != value) {
                _gameTag = value;
                OnPropertyChanged(nameof(GameTag));
            }
        }
    }

    [JsonProperty("icon")]
    public long IconId
    {
        get => _iconId;
        set {
            if (_iconId != value) {
                _iconId = value;
                OnPropertyChanged(nameof(IconId));
            }
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}