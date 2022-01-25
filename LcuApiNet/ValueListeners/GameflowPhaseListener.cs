using LcuApiNet.EventHandlers;
using LcuApiNet.Model.Enums;

namespace LcuApiNet.ValueListeners
{
    public class GameflowPhaseListener : IObservableValueListener<GameflowPhase?>
    {
        private ILcuApi _api;

        /// <inheritdoc />
        public GameflowPhase? Value { get; private set; } = null;

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
                GameflowPhase phase = await _api.Values.GetGameflowPhase();
                if (Value != phase) {
                    Value = phase;
                    ValueChanged?.Invoke(this, new ObservableValueChangedEventArgs<GameflowPhase?>(phase));
                }

                await Task.Delay(pendingRate);
            }
        }
    }
}
