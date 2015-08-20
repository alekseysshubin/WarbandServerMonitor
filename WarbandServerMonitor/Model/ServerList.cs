using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace WarbandServerMonitor.Model
{
    public class ServerList : IDisposable
    {
        private readonly string _masterServerUrl;
        private readonly TokenGenerator _tokenGenerator;

        public List<Server> Servers { get; private set; }

        public ServerList()
        {
            Servers = new List<Server>();
            _tokenGenerator = new TokenGenerator();
        }

        public ServerList(IEnumerable<Server> servers) : this()
        {
            Servers = servers.ToList();
        }

        public ServerList(string masterServerUrl) : this()
        {
            _masterServerUrl = masterServerUrl;
        }

        public void StartMonitor()
        {
            if (_masterServerUrl != null)
            {
                using (var client = new WebClient())
                {
                    var data = client.DownloadString(_masterServerUrl);
                    Servers = (
                        from serverData in data.Split('|')
                        let serverDataSplit = serverData.Split(':')
                        select new Server(
                            IPAddress.Parse(serverDataSplit[0]),                           
                            serverDataSplit.Count() > 1 ? int.Parse(serverDataSplit[1]) : 7240,
                            TimeSpan.FromSeconds(30),
                            52327,
                            _tokenGenerator)
                        ).ToList();
                }
            }
            var russian = Servers.First(x => x.IP.Equals(IPAddress.Parse("46.17.40.82")));
            Servers = new List<Server>(new[] { russian });
            foreach (var server in Servers)
            {
                server.StartMonitor();
            }
        }

        public void StopMonitor()
        {
            foreach (var server in Servers)
            {
                server.StopMonitor();
            }
        }

        public void Dispose()
        {
            foreach (var server in Servers)
            {
                server.Dispose();
            }
        }
    }
}
