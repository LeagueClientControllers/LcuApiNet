using LcuApiNet.EventHandlers;
using LcuApiNet.Model;
using System.Diagnostics;
using System.Management;

namespace LcuApiNet.Core
{
    /// <summary>
    /// Watches after the league client state and 
    /// retrieve credentials required to use league client api.
    /// </summary>
    public class LeagueClientManager : IDisposable
    {
        private const string CLIENT_EXECUTABLE_NAME = "LeagueClientUx";
        private const string CLIENT_EXECUTABLE_FILE_NAME = $"{CLIENT_EXECUTABLE_NAME}.exe";
        
        private bool _isReady = false;
        
        /// <summary>
        /// Fires when the <see cref="IsReady"/> value is changed.
        /// </summary>
        public event ClientStateChanged? StateChanged;

        /// <summary>
        /// Determines if league client is ready to receive api requests.
        /// Null value means that client location was not found
        /// and system is not able to discover client state.
        /// </summary>
        public bool IsReady { 
            get => _isReady; 
            private set {
                if (value != _isReady) {
                    _isReady = value;
                    StateChanged?.Invoke(this, new ClientStateChangedEventArgs(value));
                }
            }
        }

        /// <summary>
        /// Credentials used to access league client api.
        /// </summary>
        internal LcuCredentials? Credentials { get; private set; }

        /// <summary>
        /// Get the installation folder of the league client.
        /// This process will continue to run until league client is loaded.
        /// </summary>
        /// <param name="pendingRate">Duration of the period of time between requests in milliseconds.</param>
        internal async Task<string> FindClientLocationAsync(int pendingRate = 3000)
        {
            if (_clientLocation != null) {
                return _clientLocation;
            }


            _clientLaunched = new TaskCompletionSource();
            WatchClientLaunched(pendingRate);

            await _clientLaunched.Task.ConfigureAwait(false);
            return _clientLocation!;
        }

        /// <summary>
        /// Starts tracking state of the client.
        /// </summary>
        /// <param name="clientLocation">Path to the league client installation folder</param>
        /// <exception cref="FileNotFoundException" />
        internal async Task StartTrackingStateAsync(string clientLocation)
        {
            if (!File.Exists(Path.Combine(clientLocation, CLIENT_EXECUTABLE_FILE_NAME))) { 
                throw new FileNotFoundException($"League client not found in [{clientLocation}]");
            }

            string lockfilePath = Path.Combine(clientLocation, "lockfile");
            if (File.Exists(lockfilePath)) {
                Credentials = await ParseCredentialsAsync(lockfilePath).ConfigureAwait(false);
                IsReady = true;
            }

            _clientLockfileWatcher = new FileSystemWatcher(clientLocation, "lockfile");
            _clientLockfileWatcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastAccess;
            _clientLockfileWatcher.EnableRaisingEvents = true;
            
            _clientLockfileWatcher.Error += 
                (_, e) => throw e.GetException();

            _clientLockfileWatcher.Created += 
                async (_, _) => await LockfileCreated(lockfilePath).ConfigureAwait(false);
            
            _clientLockfileWatcher.Deleted += 
                (_, _) => LockfileDeleted();
        }

        /// <inheritdoc />
        public void Dispose()
        {
            _clientLaunchedWatcher?.Stop();
            _clientLockfileWatcher?.Dispose();
        }

        private TaskCompletionSource? _clientLaunched;
        private ManagementEventWatcher? _clientLaunchedWatcher;
        private FileSystemWatcher? _clientLockfileWatcher;

        private string? _clientLocation => 
            Path.GetDirectoryName(
                Process.GetProcessesByName(CLIENT_EXECUTABLE_NAME).FirstOrDefault()?.MainModule?.FileName); 

        private void WatchClientLaunched(int pendingRate)
        {
            WqlEventQuery query = new WqlEventQuery("__InstanceCreationEvent", 
                TimeSpan.FromMilliseconds(pendingRate), 
                $"TargetInstance ISA 'Win32_Process' AND TargetInstance.Name = '{CLIENT_EXECUTABLE_FILE_NAME}'");

            _clientLaunchedWatcher = new ManagementEventWatcher(query);
            _clientLaunchedWatcher.EventArrived += ClientLaunched;
            _clientLaunchedWatcher.Start();
        }

        private void ClientLaunched(object sender, EventArrivedEventArgs e)
        {
            _clientLaunchedWatcher!.Stop();
            _clientLaunched!.TrySetResult();
        }

        private async Task LockfileCreated(string lockfilePath)
        {
            Credentials = await ParseCredentialsAsync(lockfilePath).ConfigureAwait(false);
            IsReady = true;
        }

        private void LockfileDeleted()
        {
            Credentials = null;
            IsReady = false;
        }

        private async Task<LcuCredentials> ParseCredentialsAsync(string lockfilePath)
        {
            string lockfileContent;
            using (StreamReader lockfileReader = new StreamReader(
                    File.Open(lockfilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))) {

                lockfileContent = await lockfileReader.ReadToEndAsync().ConfigureAwait(false);
            }

            return LcuCredentials.FromString(lockfileContent);
        }
    }
}
