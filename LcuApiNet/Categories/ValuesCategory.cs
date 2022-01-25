using LcuApiNet.Exceptions;
using LcuApiNet.Model.Enums;
using System.Text.RegularExpressions;

namespace LcuApiNet.Categories
{
    /// <summary>
    /// League client api category that contains commands 
    /// that are used to retrieve some values and characteristics of the client.
    /// </summary>
    public class ValuesCategory
    {
        private ILcuApi _api;
        private Regex _rQuote = new Regex("\"");

        public ValuesCategory(ILcuApi api)
        {
            _api = api;
        }

        /// <summary>
        /// Fetch current <see cref="GameflowPhase"/> of the client.
        /// </summary>
        /// <returns>Current <see cref="GameflowPhase"/></returns>
        public async Task<GameflowPhase> GetGameflowPhase(CancellationToken token = default)
        {
            string phaseString = await _api.ExecuteAsync("lol-gameflow/v1/gameflow-phase", HttpMethod.Get, token).ConfigureAwait(false);
            
            try {
                GameflowPhase phase = (GameflowPhase)Enum.Parse(typeof(GameflowPhase), _rQuote.Replace(phaseString, ""));
                return phase;
            } catch (ArgumentException) {
                throw new WrongResponseException($"Command [lol-gameflow/v1/gameflow-phase] returned unrecognizable game flow phase - [{phaseString}]");
            } catch (OverflowException) {
                throw new WrongResponseException($"Command [lol-gameflow/v1/gameflow-phase] returned unrecognizable game flow phase - [{phaseString}]");
            }
        }
    }
}
