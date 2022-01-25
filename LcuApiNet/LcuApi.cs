using LcuApiNet.Categories;
using LcuApiNet.Core;
using LcuApiNet.Exceptions;
using LcuApiNet.Model.Enums;
using LcuApiNet.ValueListeners;
using System.Net.Http.Headers;
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

        public LcuApi()
        {
            GameflowPhaseListener = new GameflowPhaseListener(this);

            Values = new ValuesCategory(this);

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
            using (HttpClientHandler handler = new HttpClientHandler()) {
                handler.ClientCertificateOptions = ClientCertificateOption.Manual;
                handler.ServerCertificateCustomValidationCallback = (_, _, _, _) => true;
                
                using (HttpClient client = new HttpClient(handler)) {
                    client.DefaultRequestHeaders.Authorization = _requestAuthorizationHeader;
                    if (method == HttpMethod.Get) {
                        responseBody = await client.GetStringAsync(url, token).ConfigureAwait(false);
                    } else if (method == HttpMethod.Post) {
                    
                    } else if (method == HttpMethod.Put) {

                    } else if (method == HttpMethod.Delete) {

                    } else if (method == HttpMethod.Head) { 
                
                    } else if (method == HttpMethod.Patch) {

                    } else if (method == HttpMethod.Options) {

                    } else if (method == HttpMethod.Trace) {

                    }
                }
            }

            return responseBody!;
        }
    }
}
