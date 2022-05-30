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
}