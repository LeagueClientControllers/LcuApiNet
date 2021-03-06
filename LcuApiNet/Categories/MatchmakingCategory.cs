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
                await _api.Socket.ExecuteAsync("/lol-matchmaking/v1/ready-check/accept", HttpMethod.Post);
            } catch (ApiCommandException e) {
                if (e.Details.Code == ErrorCode.InternalError && (
                    e.Details.Message == "Not attached to a matchmaking queue." ||
                    e.Details.Message == "Failed to accept team builder ready check: Error response for POST /lol-lobby-team-builder/v1/ready-check/accept: Cannot accept team builder AFK check because the current phase is: MATCHMAKING")) {
                    throw new NotAttachedToQueueException();
                }

                throw new WrongResponseException("When accepting a match wrong exception arrived.");
            }
        }

        public async Task DeclineMatchAsync(CancellationToken token = default)
        {
            try {
                await _api.Socket.ExecuteAsync("/lol-matchmaking/v1/ready-check/decline", HttpMethod.Post);
            } catch (ApiCommandException e) {
                if (e.Details.Code == ErrorCode.InternalError && (
                    e.Details.Message == "Not attached to a matchmaking queue." || 
                    e.Details.Message == "Failed to decline team builder ready check: Error response for POST /lol-lobby-team-builder/v1/ready-check/decline: Cannot decline team builder AFK check because the current phase is: MATCHMAKING")) {
                    throw new NotAttachedToQueueException();
                }

                throw new WrongResponseException("When declining a match wrong exception arrived.");
            }
        }
        
        public async Task StartMatchmaking(CancellationToken token = default)
        {
            await _api.Socket.ExecuteAsync("/lol-lobby/v2/lobby/matchmaking/search", HttpMethod.Post);
        }
        
        public async Task StopMatchmaking(CancellationToken token = default)
        {
            await _api.Socket.ExecuteAsync("/lol-lobby/v2/lobby/matchmaking/search", HttpMethod.Delete);
        }
    }
}
