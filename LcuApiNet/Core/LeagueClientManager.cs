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
        private ILcuApi _api;
        
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

        public LeagueClientManager(ILcuApi api)
        {
            _api = api;
        }

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
            if (File.Exists(lockfilePath) && Process.GetProcessesByName(CLIENT_EXECUTABLE_NAME).Length != 0) {
                Credentials = await ParseCredentialsAsync(lockfilePath).ConfigureAwait(false);
                await _api.Socket.ConnectAsync(Credentials).ConfigureAwait(false);
                _api.LeagueEvents.ResubscribeAll();
                IsReady = true;
            }

            _clientLockfileWatcher = new FileSystemWatcher(clientLocation, "lockfile");
            _clientLockfileWatcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastAccess;
            _clientLockfileWatcher.EnableRaisingEvents = true;
        
            _clientLockfileWatcher.Error += 
                (_, e) => throw e.GetException();

            _clientLockfileWatcher.Created += 
                async (_, _) => await LockfileCreated(lockfilePath).ConfigureAwait(false);

            _clientLockfileWatcher.Changed += 
                async (_, _) => await LockfileChanged(lockfilePath).ConfigureAwait(false);
        
            _clientLockfileWatcher.Deleted += 
                async (_, _) => await LockfileDeleted().ConfigureAwait(false);
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
            Console.WriteLine("Lock file created");
            Credentials = await ParseCredentialsAsync(lockfilePath).ConfigureAwait(false);
            await _api.Socket.ConnectAsync(Credentials).ConfigureAwait(false);
            _api.LeagueEvents.ResubscribeAll();
            IsReady = true;
        }
        
        private async Task LockfileChanged(string lockfilePath)
        {
            Console.WriteLine("Lock file changed");
            if (!IsReady) {
                Credentials = await ParseCredentialsAsync(lockfilePath).ConfigureAwait(false);
                await _api.Socket.ConnectAsync(Credentials).ConfigureAwait(false);
                _api.LeagueEvents.ResubscribeAll();
                IsReady = true;
            }
        }
        
        private async Task LockfileDeleted()
        {
            Console.WriteLine("Lock file deleted");
            Credentials = null;
            await _api.Socket.DisconnectAsync().ConfigureAwait(false);
            IsReady = false;
        }

        private async Task<LcuCredentials> ParseCredentialsAsync(string lockfilePath)
        {
            string lockfileContent;
            using (StreamReader lockfileReader = new StreamReader(
                    File.Open(lockfilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))) {

                lockfileContent = await lockfileReader.ReadToEndAsync().ConfigureAwait(false);
            }

            Console.WriteLine($"[ParseCredentialsAsync] lockfileContent = {lockfileContent}");

            return LcuCredentials.FromString(lockfileContent);
        }
    }
}
