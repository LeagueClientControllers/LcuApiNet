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
}