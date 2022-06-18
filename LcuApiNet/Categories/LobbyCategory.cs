using LcuApiNet.Model.Enums;
using Newtonsoft.Json.Linq;

namespace LcuApiNet.Categories;

public class LobbyCategory
{
    private ILcuApi _api;

    public LobbyCategory(ILcuApi api)
    {
        _api = api;
    }

    public async void CreateLobby(int queueId, CancellationToken token = default)
    {
        await _api.Socket.ExecuteAsync($"/lol-lobby/v2/lobby", HttpMethod.Post,$"{{\"queueId\": {queueId}}}", token: token).ConfigureAwait(false);
    }

    public async Task<QueueType> GetQueueType(CancellationToken token = default)
    {
        JObject responseObject = JObject.Parse(await _api.Socket.ExecuteAsync($"/lol-lobby/v2/lobby", HttpMethod.Get, token: token));
        return QueueType.FromValue(Convert.ToInt32(((JValue)responseObject.SelectToken("gameConfig")!["queueId"]!).Value));
    }
}