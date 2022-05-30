using LcuApiNet.Categories;
using LcuApiNet.Core;
using LcuApiNet.Exceptions;
using LcuApiNet.Model;
using LcuApiNet.Model.Enums;

using Newtonsoft.Json;
using Websocket.Client;

using System.Diagnostics;
using System.Net;
using System.Net.Http.Headers;
using System.Net.WebSockets;
using System.Net.Sockets;
using System.Reactive.Linq;
using System.Text;
using LcuApiNet.Utilities;
using Newtonsoft.Json.Linq;
using LcuApiNet.Core.Events;
using LcuApiNet.EventHandlers;

namespace LcuApiNet
{
    /// <inheritdoc />
    public class LcuApi : ILcuApi
    {
        /// <inheritdoc />
        public LeagueClientManager Client { get; private set; }

        /// <inheritdoc />
        public WampSocketManager Socket { get; }

        /// <inheritdoc />
        public ValuesCategory Values { get; }

        /// <inheritdoc />
        public MatchmakingCategory Matchmaking { get; }

        public LobbyCategory Lobby { get; }

        public SummonerCategory Summoner { get; }

        /// <inheritdoc />
        public LeagueEventService LeagueEvents { get; }
        
        public CustomEventService CustomEvents { get; }
        
        public PickCategory Pick { get;  }

        public LcuApi()
        {
            Values       = new ValuesCategory(this);
            Matchmaking  = new MatchmakingCategory(this);
            Lobby        = new LobbyCategory(this);
            Summoner     = new SummonerCategory(this);
            Socket       = new WampSocketManager();
            Client       = new LeagueClientManager(this);
            LeagueEvents = new LeagueEventService(this);
            CustomEvents = new CustomEventService(this);
            Pick         = new PickCategory(this);
        }  


        /// <inheritdoc />
        public async Task InitAsync(string? clientLocation = null, CancellationToken token = default)
        {
            if (clientLocation != null) {
                try {
                    await Client.StartTrackingStateAsync(clientLocation).ConfigureAwait(false);
                }
                catch (FileNotFoundException) {
                    _ = Task.Run(async () => {
                        clientLocation = await Client.FindClientLocationAsync();
                        await Client.StartTrackingStateAsync(clientLocation);
                    }, token).ContinueWith(t => throw t.Exception!, TaskContinuationOptions.OnlyOnFaulted);
                }
            } else {
                _ = Task.Run(async () => {
                    clientLocation = await Client.FindClientLocationAsync().ConfigureAwait(false);
                    await Client.StartTrackingStateAsync(clientLocation).ConfigureAwait(false);
                }, token).ContinueWith(t => Console.WriteLine(t.Exception!), TaskContinuationOptions.OnlyOnFaulted).ConfigureAwait(false);
            }
        }

    }
}
