using LcuApiNet.Model.ClientModels;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LcuApiNet.Categories;

public class SummonerCategory
{
    private ILcuApi _api;

    public SummonerCategory(ILcuApi api)
    {
        _api = api;
    }
    
    public async Task<long> GetLocalUserId(CancellationToken token = default)
    {
        JObject responseArray = JObject.Parse(await _api.Socket.ExecuteAsync("/lol-summoner/v1/current-summoner", HttpMethod.Get, token: token)
            .ConfigureAwait(false));
        return Convert.ToInt64(((JValue)responseArray.SelectToken("summonerId")!).Value!);
    }

    public async Task<Summoner> GetLocalUserInfo(CancellationToken token = default)
    {
        JObject responseArray = JObject.Parse(await _api.Socket.ExecuteAsync("/lol-chat/v1/me", HttpMethod.Get, token: token)
            .ConfigureAwait(false));

        string gameName = ((JValue) responseArray.SelectToken("gameName")!).Value!.ToString()!;
        string gameTag = ((JValue) responseArray.SelectToken("gameTag")!).Value!.ToString()!;
        long icon = Convert.ToInt64(((JValue) responseArray.SelectToken("icon")!).Value!);
        
        return JsonConvert.DeserializeObject<Summoner>($"{{ \"gameName\":\"{gameName}\", \"gameTag\":\"{gameTag}\", \"icon\": \"{icon}\"}}")!;
    }  
}