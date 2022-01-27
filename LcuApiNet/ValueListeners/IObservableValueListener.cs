using LcuApiNet.EventHandlers;

namespace LcuApiNet.ValueListeners
{
    /// <summary>
    /// Interface for modules that has some value of the <typeparamref name="V"/>
    /// and provide facilities for listening and handling value changes.
    /// </summary>
    /// <typeparam name="TValue">Type of the value stored.</typeparam>
    public interface IObservableValueListener<TValue>
    {
        /// <summary>
        /// Current value.
        /// </summary>
        TValue Value { get; }

        /// <summary>
        /// Fires when <see cref="Value"/> is changed;
        /// </summary>
        event ObservableValueChanged<TValue>? ValueChanged;

        /// <summary>
        /// Starts process that will be listening the value to change.
        /// </summary>
        /// <param name="pendingRate">Duration of the period of time between requests in milliseconds.</param>
        /// <param name="token">Token that allows to cancel current task and listening process</param>
        Task StartListeningAsync(int pendingRate = 200, CancellationToken token = default);
    }
}
