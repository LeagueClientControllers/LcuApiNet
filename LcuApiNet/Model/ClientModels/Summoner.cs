using Newtonsoft.Json;

namespace LcuApiNet.Model.ClientModels;
using Prism.Mvvm;

public class Summoner : BindableBase
{
    private string _name = default!;
    private string _gameTag = default!;
    private long _iconId;
    
    [JsonProperty("gameName")]
    public string Name { 
        get => _name;
        set => SetProperty(ref _name, value);
    }
    
    [JsonProperty("gameTag")]
    public string GameTag {
        get => _gameTag;
        set => SetProperty(ref _gameTag, value);
    }

    [JsonProperty("icon")]
    public long IconId
    {
        get => _iconId;
        set => SetProperty(ref _iconId, value);
    }
    
}