using LcuApiNet.Categories;
using LcuApiNet.Core;
using LcuApiNet.Exceptions;
using LcuApiNet.Model;
using LcuApiNet.Model.Enums;
using LcuApiNet.ValueListeners;

using Newtonsoft.Json;

using System.Diagnostics;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Sockets;
using System.Text;

namespace LcuApiNet
{
    /// <inheritdoc />
    public class LcuApi : ILcuApi
    {
        private Uri? _baseUrl;
        private AuthenticationHeaderValue? _requestAuthorizationHeader;

        /// <inheritdoc />
        public LeagueClientManager Client { get; private set; }

        /// <inheritdoc />
        public IObservableValueListener<GameflowPhase?> GameflowPhaseListener { get; }

        /// <inheritdoc />
        public ValuesCategory Values { get; }

        /// <inheritdoc />
        public MatchmakingCategory Matchmaking { get; }

        public LcuApi()
        {
            GameflowPhaseListener = new GameflowPhaseListener(this);

            Values = new ValuesCategory(this);
            Matchmaking = new MatchmakingCategory(this);

            Client = new LeagueClientManager();
            Client.StateChanged += (o, e) => {
                if (e.State) {
                    _baseUrl = new Uri($"{Client.Credentials!.Protocol}://127.0.0.1:{Client.Credentials!.Port}");
                    _requestAuthorizationHeader = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.GetEncoding("ISO-8859-1").GetBytes($"riot:{Client.Credentials!.Password}")));
                }
            };
        }

        /// <inheritdoc />
        public async Task InitAsync(string? clientLocation = null, CancellationToken token = default)
        {
            if (clientLocation != null) {
                try {
                    await Client.StartTrackingStateAsync(clientLocation).ConfigureAwait(false);
                } catch (FileNotFoundException) {
                    _ = Task.Run(async () => {
                        clientLocation = await Client.FindClientLocationAsync();
                        await Client.StartTrackingStateAsync(clientLocation);
                    }).ContinueWith(t => throw t.Exception!, TaskContinuationOptions.OnlyOnFaulted);
                }
            } else {
                _ = Task.Run(async () => {
                    clientLocation = await Client.FindClientLocationAsync();
                    await Client.StartTrackingStateAsync(clientLocation);
                }).ContinueWith(t => Console.WriteLine(t.Exception!), TaskContinuationOptions.OnlyOnFaulted);
            }
        }

        /// <inheritdoc />
        public async Task<string> ExecuteAsync(string commandPath, HttpMethod method, CancellationToken token = default)
        {
            if (!Client.IsReady) {
                throw new ClientNotReadyException();
            }

            string? responseBody = null;
            Uri url = new Uri(_baseUrl!, commandPath);
            HttpRequestMessage requestMessage = new HttpRequestMessage(method, url);
            using (HttpClientHandler handler = new HttpClientHandler()) {
                handler.ClientCertificateOptions = ClientCertificateOption.Manual;
                handler.ServerCertificateCustomValidationCallback = (_, _, _, _) => true;
                
                using (HttpClient client = new HttpClient(handler)) {
                    client.DefaultRequestHeaders.Authorization = _requestAuthorizationHeader;
                    try {
                        HttpResponseMessage response = await client.SendAsync(requestMessage);
                        responseBody = await response.Content.ReadAsStringAsync(token).ConfigureAwait(false);
                        
                        if (!response.IsSuccessStatusCode) {
                            ApiError? error = JsonConvert.DeserializeObject<ApiError>(responseBody);
                            if (error == null) {
                                throw new WrongResponseException("Request completed with unsuccessful status code but no error details provided");
                            }

                            throw new ApiCommandException(error);
                        }
                    } catch (HttpRequestException e) {
                        if (e.InnerException is SocketException sE && sE.SocketErrorCode == SocketError.ConnectionRefused) {
                            throw new ApiServerUnreachableException();
                        }

                        throw e;
                    }
                }
            }

            return responseBody!;
        }
    }
}
