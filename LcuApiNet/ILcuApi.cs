using LcuApiNet.Categories;
using LcuApiNet.Core;
using LcuApiNet.Model.Enums;
using LcuApiNet.ValueListeners;

namespace LcuApiNet
{
    /// <summary>
    /// Core class of the api.
    /// </summary>
    public interface ILcuApi
    {
        /// <summary>
        /// Contains information about the league client.
        /// </summary>
        LeagueClientManager Client { get; }

        /// <summary>
        /// Listener of the game flow phase.
        /// </summary>
        IObservableValueListener<GameflowPhase?> GameflowPhaseListener { get; }

        /// <summary>
        /// Api category for retrieve client values.
        /// </summary>
        ValuesCategory Values { get; }
        
        /// <summary>
        /// Api category for matchmaking control.
        /// </summary>
        MatchmakingCategory Matchmaking { get; }

        /// <summary>
        /// Initializes api module.
        /// Starts tracking league client state.
        /// If <paramref name="clientLocation"/> was invalid or not provided 
        /// starts finding client location (the process will not wait for client to be found).
        /// </summary>
        /// <param name="clientLocation">Predefined path to the league client installation directory.</param>
        Task InitAsync(string? clientLocation = null, CancellationToken token = default);

        /// <summary>
        /// Executes client api command with string response.
        /// </summary>
        /// <param name="methodPath">Relative path to the client command.</param>
        /// <param name="method">HTTP method required.</param>
        /// <exception cref="LcuApiNet.Exceptions.ClientNotReadyException"></exception>
        /// <exception cref="LcuApiNet.Exceptions.ApiServerUnreachableException"></exception>
        /// <exception cref="LcuApiNet.Exceptions.WrongResponseException"></exception>
        /// <exception cref="LcuApiNet.Exceptions.ApiCommandException"></exception>
        /// <returns>Command string response.</returns>
        Task<string> ExecuteAsync(string commandPath, HttpMethod method, CancellationToken token = default);
    }
}
