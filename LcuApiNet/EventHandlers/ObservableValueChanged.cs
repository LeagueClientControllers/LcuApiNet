using LcuApiNet.ValueListeners;

namespace LcuApiNet.EventHandlers
{

    /// <summary>
    /// Represents function that will handle <see cref="IObservableValueListener{TValue}.ValueChanged"/> event.
    /// </summary>
    public delegate void ObservableValueChanged<TValue>(object sender, ObservableValueChangedEventArgs<TValue> e);

    /// <summary>
    /// Provides data for the <see cref="IObservableValueListener{TValue}.ValueChanged"/> event.
    /// </summary>
    /// <typeparam name="TValue">Type of the changed value.</typeparam>
    public class ObservableValueChangedEventArgs<TValue>
    {
        /// <summary>
        /// Changed value.
        /// </summary>
        public TValue NewValue { get; }

        /// <summary>
        /// Creates new arguments for the <see cref="IObservableValueListener{TValue}.ValueChanged"/> events.
        /// </summary>
        /// <param name="value">Changed value</param>
        public ObservableValueChangedEventArgs(TValue value)
        {
            NewValue = value;
        }
    }
}
