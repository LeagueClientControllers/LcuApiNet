using System.Net;
using System.Net.WebSockets;
using System.Reactive.Linq;
using LcuApiNet.Core.Events;
using LcuApiNet.Exceptions;
using LcuApiNet.Model;
using LcuApiNet.Model.Enums;
using LcuApiNet.Utilities;
using Newtonsoft.Json.Linq;
using Websocket.Client;


namespace LcuApiNet.Core;

public class WampSocketManager : IAsyncDisposable
{
    private TaskCompletionSource<string>? _receiveExecutionResult;

    private TaskCompletionSource? _receiveSessionId;
    
    private WebsocketClient _webSocket = null!;
    private string _sessionId = null!;
    private Uri _serverUrl = null!;
    private Action<string>? _wampEventHandler;
    public async Task ConnectAsync(LcuCredentials credentials, CancellationToken token = default)
    {
        _serverUrl = new Uri($"wss://127.0.0.1:{credentials.Port}");

        _webSocket = new WebsocketClient(_serverUrl, () => {
            ClientWebSocket socket = new ClientWebSocket {
                Options = {
                    Credentials = new NetworkCredential("riot", credentials.Password),
                    RemoteCertificateValidationCallback = (_, _, _, _) => true,
                    
                }
            };
            
            socket.Options.AddSubProtocol("wamp");
            
            return socket;
        });

        _webSocket.ReconnectTimeout = TimeSpan.MaxValue;
        _webSocket.ErrorReconnectTimeout = TimeSpan.FromSeconds(10);
        
        _receiveSessionId = new TaskCompletionSource();
        _webSocket.MessageReceived.Subscribe(MessageReceived);

        await _webSocket.Start().ConfigureAwait(false);
        await _receiveSessionId.Task.ConfigureAwait(false);

        _receiveSessionId = null;
    }

    public void Subscribe(string eventCategory)
    {
        EnsureWebSocketAvailable();
        _webSocket.Send($"[{(int)WampMessageType.Subscribe},\"{eventCategory}\"]");
    }

    //public void Subscribe(string eventCategory, LeagueInternalEventType leagueInternalEventType, string eventUri)
    //{
    //    _webSocket.Send($"[{(int)WampMessageType.Subscribe},\"{eventCategory}\",{{\"data\":[],\"eventType\":\"{leagueInternalEventType}\",\"uri\":\"{eventUri}\"}}]");
    //}

    public void Unsubscribe(string eventCategory)
    {
        EnsureWebSocketAvailable();
        _webSocket.Send($"[{(int)WampMessageType.Unsubscribe}, \"{eventCategory}\"]");
    }

    public void AttachWampEventHandler(Action<string> action)
    {
        _wampEventHandler = action;
    }

    public async Task<string> ExecuteAsync(string commandPath, HttpMethod method, string? payload = null, CancellationToken token = default)
    {
        EnsureWebSocketAvailable();
        if (_receiveExecutionResult != null && !_receiveExecutionResult.Task.IsFaulted) {
            throw new InvalidOperationException("Unable to execute method while previous execution wasn't completed.");
        }

        _receiveExecutionResult = new TaskCompletionSource<string>();
        _webSocket.Send(payload == null
            ? $"[{(int) WampMessageType.Call}, \"{_sessionId}\", \"{method.Method} {commandPath}\"]"
            : $"[{(int) WampMessageType.Call}, \"{_sessionId}\", \"{method.Method} {commandPath}\", {payload}]");

        token.Register(() => { _receiveExecutionResult.SetCanceled(token); }); 
        string response = await _receiveExecutionResult.Task.ConfigureAwait(false);
        _receiveExecutionResult = null;
        return response;
    }

    private void MessageReceived(ResponseMessage message)
    {
        //Console.WriteLine(message.Text);
        JArray? eventArray = JArray.Parse(message.Text);
        if (eventArray == null) {
            throw new WampSocketExсeption("Error when parsing league wamp message");
        }

        WampMessageType eventNumber = (WampMessageType)eventArray[0].ToObject<int>();
        if (eventNumber == WampMessageType.Welcome) {
            _sessionId = eventArray[1].ToString();
            _receiveSessionId?.TrySetResult();
        } else if (eventNumber == WampMessageType.CallResult) {
            Task.Run(() => _receiveExecutionResult?.TrySetResult(eventArray[2].ToString()));
        } else if (eventNumber == WampMessageType.Event) {
            _wampEventHandler?.Invoke(eventArray[2].ToString());
        } else if (eventNumber == WampMessageType.CallError) {
            ApiError apiError = new ApiError(Enum.Parse<ErrorCode>(eventArray[2].ToString()), eventArray[3].ToString());
            Task.Run(() => _receiveExecutionResult?.TrySetException(new ApiCommandException(apiError)));       
        }
    }

    private void EnsureWebSocketAvailable()
    {
        if(_webSocket == null || !_webSocket.IsRunning)
        {
            throw new ClientNotReadyException();
        }
    }

    public async Task DisconnectAsync(CancellationToken token = default)
    {
        await _webSocket.Stop(WebSocketCloseStatus.NormalClosure, "League client closed").ConfigureAwait(false);
    }

    public async ValueTask DisposeAsync()
    {
        await DisconnectAsync().ConfigureAwait(false);
    }
}