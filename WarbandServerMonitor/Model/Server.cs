using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Timers;
using System.Xml.Linq;
using WarbandServerMonitor.Annotations;

namespace WarbandServerMonitor.Model
{
    public class Server : IDisposable, INotifyPropertyChanged
    {
        #region Fields

        private readonly Timer _monitorTimer;
        private readonly Ping _pinger;

        private int _playersCount;
        private string _name;
        private string _module;
        private int _version;
        private string _map;
        private string _gameType;
        private int _maxPlayers;
        private bool _hasPassword;
        private bool _isDedicated;
        private long? _ping;
        private bool _isLoaded;

        #endregion Fields

        #region Properties

        public IPAddress IP { get; private set; }
        public int Port { get; private set; }

        public string Name
        {
            get { return _name; }
            private set { SetProperty(ref _name, value); }
        }

        public string Module
        {
            get { return _module; }
            private set { SetProperty(ref _module, value); }
        }

        public int Version
        {
            get { return _version; }
            private set { SetProperty(ref _version, value); }
        }

        public string Map
        {
            get { return _map; }
            private set { SetProperty(ref _map, value); }
        }

        public string GameType
        {
            get { return _gameType; }
            private set { SetProperty(ref _gameType, value); }
        }

        public int PlayersCount
        {
            get { return _playersCount; }
            private set { SetProperty(ref _playersCount, value); }
        }

        public int MaxPlayers
        {
            get { return _maxPlayers; }
            private set { SetProperty(ref _maxPlayers, value); }
        }

        public bool HasPassword
        {
            get { return _hasPassword; }
            private set { SetProperty(ref _hasPassword, value); }
        }

        public bool IsDedicated
        {
            get { return _isDedicated; }
            private set { SetProperty(ref _isDedicated, value); }
        }

        public long? Ping
        {
            get { return _ping; }
            private set { SetProperty(ref _ping, value); }
        }

        public bool IsLoaded
        {
            get { return _isLoaded; }
            private set { SetProperty(ref _isLoaded, value); }
        }

        #endregion Properties

        public Server(IPAddress ip, int port, TimeSpan interval, int localPort, TokenGenerator tokenGenerator)
        {
            _pinger = new Ping(localPort, new IPEndPoint(ip, port), tokenGenerator);
            IP = ip;
            Port = port;
            _monitorTimer = new Timer(interval.TotalMilliseconds);
            _monitorTimer.Elapsed += (_, __) => Load();
        }

        #region Methods

        public void StartMonitor()
        {
            Task.Run(async () => await Load()).ContinueWith(_ => _monitorTimer.Start());
        }

        public void StopMonitor()
        {
            _monitorTimer.Stop();
        }

        private async Task Load()
        {
            try
            {
                var client = new TcpClient();
                await client.ConnectAsync(IP, Port);
                using (var reader = new StreamReader(client.GetStream()))
                {
                    var data = await reader.ReadToEndAsync();
                    var xml = XElement.Parse(data);
                    Name = xml.Descendants("Name").First().Value;
                    Module = xml.Descendants("ModuleName").First().Value;
                    Version = int.Parse(xml.Descendants("MultiplayerVersionNo").First().Value);
                    Map = xml.Descendants("MapName").First().Value;
                    GameType = xml.Descendants("MapTypeName").First().Value;
                    PlayersCount = int.Parse(xml.Descendants("NumberOfActivePlayers").First().Value);
                    MaxPlayers = int.Parse(xml.Descendants("MaxNumberOfPlayers").First().Value);
                    HasPassword = xml.Descendants("HasPassword").First().Value.ToLower() == "yes";
                    IsDedicated = xml.Descendants("IsDedicated").First().Value.ToLower() == "yes";
                }
                Ping = await _pinger.Send();
                IsLoaded = true;
            }
            catch { /* Something wrong with the server. Skip. */ }
        }

        public void Dispose()
        {
            StopMonitor();
            _monitorTimer.Dispose();
        }

        #endregion Methods

        #region PropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        protected virtual void SetProperty<T>(ref T value, T newValue, [CallerMemberName] string propertyName = null)
        {
            if (Equals(value, newValue)) return;
            value = newValue;
            OnPropertyChanged(propertyName);
        }

        #endregion PropertyChanged
    }
}
