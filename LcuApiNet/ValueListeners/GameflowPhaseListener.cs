using LcuApiNet.EventHandlers;
using LcuApiNet.Exceptions;
using LcuApiNet.Model.Enums;

namespace LcuApiNet.ValueListeners
{
    public class GameflowPhaseListener : IObservableValueListener<GameflowPhase?>
    {
        private ILcuApi _api;
        private GameflowPhase? _value = null;

        /// <inheritdoc />
        public GameflowPhase? Value { 
            get => _value;
            private set {
                if (value != _value) {
                    _value = value;
                    ValueChanged?.Invoke(this, new ObservableValueChangedEventArgs<GameflowPhase?>(value));
                }
            }
        }

        /// <inheritdoc />
        public event ObservableValueChanged<GameflowPhase?>? ValueChanged;

        public GameflowPhaseListener(ILcuApi api)
        {
            _api = api;
        }

        /// <inheritdoc />
        public async Task StartListeningAsync(int pendingRate = 250, CancellationToken token = default)
        {
            _ = Task.Run(async () => {
                await ListeningProcess(pendingRate, token);
            }).ContinueWith(t => Console.WriteLine(t.Exception!), TaskContinuationOptions.OnlyOnFaulted);
        }

        private async Task ListeningProcess(int pendingRate, CancellationToken token = default)
        {
            while (!token.IsCancellationRequested) {
                try {
                    GameflowPhase phase = await _api.Values.GetGameflowPhase();
                    if (Value != phase) {
                        Value = phase;
                    }
                } catch (ApiServerUnreachableException) {
                    if (Value != null) {
                        Value = null;
                    }
                }

                await Task.Delay(pendingRate);
            }

            Value = null;
        }
    }
}
