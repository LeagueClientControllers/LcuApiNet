using System.Diagnostics;
using System.Net.Http.Json;
using LcuApiNet.EventHandlers.PickStage;
using LcuApiNet.Exceptions;
using LcuApiNet.Model;
using LcuApiNet.Model.ClientModels;
using LcuApiNet.Model.ClientModels.ChampSelectModels;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LcuApiNet.Categories;

/// <summary>
/// Contains method that are related to the league pick  process.
/// </summary>
public class PickCategory
{
    private ILcuApi _api;

    public PickCategory(ILcuApi api)
    {
        _api = api;
    }

    public async Task<string> GetAllChampions(CancellationToken token = default)
    {
        return await _api.Socket.ExecuteAsync("/lol-champ-select/v1/all-grid-champions", HttpMethod.Get, token: token).ConfigureAwait(false);
    }
    
    public async Task LockChampion(int sessionActionId, CancellationToken token = default)
    {
        try {
            await _api.Socket.ExecuteAsync($"/lol-champ-select/v1/session/actions/{sessionActionId}/complete", HttpMethod.Post, token: token).ConfigureAwait(false);
        } catch (ApiCommandException e) {
            Debug.WriteLine(e.Message);
        }
    }

    public async Task HoverChampion(int sessionActionId, int championId,  Action? callback = null, CancellationToken token = default)
    {
        ChampionHoveredHandler? handler = null;
        
        handler = (_, args) => {
            if (sessionActionId == args.SessionActionId) {
                _api.CustomEvents.ChampionHovered -= handler;
                callback?.Invoke();
            }
        };

        _api.CustomEvents.ChampionHovered += handler;
        
        try {
            await _api.Socket.ExecuteAsync($"/lol-champ-select/v1/session/actions/{sessionActionId}", HttpMethod.Patch,
                $"{{\"championId\": {championId}}}", token).ConfigureAwait(false);
        } catch (ApiCommandException e) {
            Debug.WriteLine(e.Message);
        }
    }
    
    public async Task<ChampSelectSession> GetSessionInfoAsync(CancellationToken token = default)
    {
        string responseString = await _api.Socket.ExecuteAsync($"/lol-champ-select/v1/session", HttpMethod.Get, token: token)
            .ConfigureAwait(false);
        return JsonConvert.DeserializeObject<ChampSelectSession>(responseString)!;
    }

    public async Task<int[]> GetAvailableChampionIds(CancellationToken token = default)
    {
        JArray responseArray = JArray.Parse(await _api.Socket.ExecuteAsync($"/lol-champ-select/v1/all-grid-champions",
            HttpMethod.Get, token: token));

        responseArray.Remove(responseArray[0]);

        return (from t in responseArray where (bool) ((JValue) t.SelectToken("owned")!).Value! select Convert.ToInt32(((JValue) t.SelectToken("id")!).Value!)).ToArray();
    }
    
    public async Task<int> GetUserActionIdAsync(CancellationToken token = default)
    {
        ChampSelectSession sessionInfo = await GetSessionInfoAsync(token).ConfigureAwait(false);
        long userId = await _api.Summoner.GetLocalUserId(token).ConfigureAwait(false);
        int userCellId = sessionInfo.MyTeam.Where(x => x.SummonerId == userId).ToArray()[0].CellId;
        int userActionId = 0;
        
        foreach (SessionAction[] actions in sessionInfo.Actions) {
            foreach (SessionAction action in actions) {
                if (action.ActorCellId == userCellId) {
                    userActionId = action.Id;
                }
            }
        }

        return userActionId;
    }

}
