namespace LcuApiNet.EventHandlers
{
    /// <summary>
    /// Represents function that will handle <see cref="LcuApiNet.Core.LeagueClientManager.StateChanged"/> event.
    /// </summary>
    public delegate void ClientStateChanged(object sender, ClientStateChangedEventArgs e);

    /// <summary>
    /// Provides data for the <see cref="LcuApiNet.Core.LeagueClientManager.StateChanged"/> event.
    /// </summary>
    public class ClientStateChangedEventArgs
    {
        /// <summary>
        /// New client state.
        /// </summary>
        public bool State { get; }

        /// <summary>
        /// Creates arguments for the <see cref="LcuApiNet.Core.LeagueClientManager.StateChanged"/> event.
        /// </summary>
        /// <param name="state"></param>
        public ClientStateChangedEventArgs(bool state) =>
            State = state;
    }
}
