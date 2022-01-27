using LcuApiNet.Exceptions;
using LcuApiNet.Model;

namespace LcuApiNet.Categories
{
    /// <summary>
    /// Contains method that are related to the league matchmaking process.
    /// </summary>
    public class MatchmakingCategory
    {
        private ILcuApi _api;

        public MatchmakingCategory(ILcuApi api)
        {
            _api = api;
        }

        public async Task AcceptMatchAsync(CancellationToken token = default)
        {
            try {
                await _api.ExecuteAsync("lol-matchmaking/v1/ready-check/accept", HttpMethod.Post, token);
            } catch (ApiCommandException e) {
                if (e.Details.Code == ErrorCode.RPC_ERROR && e.Details.Message == "Not attached to a matchmaking queue.") {
                    throw new NotAttachedToQueueException();
                }

                throw new WrongResponseException("When accepting a match only RPC error can occur");
            }
        }

        public async Task DeclineMatchAsync(CancellationToken token = default)
        {
            try {
                await _api.ExecuteAsync("lol-matchmaking/v1/ready-check/decline", HttpMethod.Post, token);
            } catch (ApiCommandException e) {
                if (e.Details.Code == ErrorCode.RPC_ERROR && e.Details.Message == "Not attached to a matchmaking queue.") {
                    throw new NotAttachedToQueueException();
                }

                throw new WrongResponseException("When declining a match only RPC error can occur");
            }
        }
    }
}
