using LcuApiNet.Categories;
using LcuApiNet.Core;
using LcuApiNet.Core.Events;
using LcuApiNet.EventHandlers;
using LcuApiNet.Model.Enums;

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
        /// 
        /// </summary>
        WampSocketManager Socket { get; }

        /// <summary>
        /// Api category for retrieve client values.
        /// </summary>
        ValuesCategory Values { get; }
        
        /// <summary>
        /// Api category for matchmaking control.
        /// </summary>
        MatchmakingCategory Matchmaking { get; }

        LobbyCategory Lobby { get;  }

        SummonerCategory Summoner { get; }

        /// <summary>
        /// 
        /// </summary>
        LeagueEventService LeagueEvents { get;  }
        
        CustomEventService CustomEvents { get; }
        
        /// <summary>
        /// Api category for pick stage control. 
        /// </summary>
        PickCategory Pick { get;  }

        /// <summary>
        /// Initializes api module.
        /// Starts tracking league client state.
        /// If <paramref name="clientLocation"/> was invalid or not provided 
        /// starts finding client location (the process will not wait for client to be found).
        /// </summary>
        /// <param name="clientLocation">Predefined path to the league client installation directory.</param>
        Task InitAsync(string? clientLocation = null, CancellationToken token = default);
    }
}
