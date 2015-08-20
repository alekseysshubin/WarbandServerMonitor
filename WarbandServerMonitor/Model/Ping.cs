using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using WarbandServerMonitor.Common;

namespace WarbandServerMonitor.Model
{
    public class Ping
    {
        private readonly TokenGenerator _tokenGenerator;
        private readonly int _localPort;

        public IPEndPoint Server { get; private set; }

        public Ping(int localPort, IPEndPoint serverEndpoint, TokenGenerator tokenGenerator)
        {
            _localPort = localPort;
            Server = serverEndpoint;
            _tokenGenerator = tokenGenerator;
        }

        public async Task<long?> Send()
        {
            Int16 token = 0;
            UdpClient client = null;
            Stopwatch stopwatch = null;
            try
            {
                token = _tokenGenerator.Get();
                var tokenBytes = BitConverter.GetBytes(token);
                var bytes = new byte[] { 0x06, tokenBytes[0], tokenBytes[1], 0x00, 0x00, 0x01 };

                //client = new UdpClient(_localPort) { ExclusiveAddressUse = false };
                client = new UdpClient();
                client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                client.Client.Bind(new IPEndPoint(IPAddress.Any, _localPort));
                stopwatch = new Stopwatch();
                var result = await Task.Run(async () => await SendPing(client, stopwatch, token, bytes))
                    .WithTimeout(TimeSpan.FromSeconds(1));

                if (result.Success) return stopwatch.ElapsedMilliseconds;
                else throw new TimeoutException();
            }
            finally
            {
                if (token != 0) _tokenGenerator.Free(token);
                if (client != null) client.Close();
                if (stopwatch != null) stopwatch.Stop();
            }
        }

        private async Task SendPing(UdpClient client, Stopwatch stopwatch, Int16 token, byte[] bytes)
        {
            stopwatch.Start();
            await client.SendAsync(bytes, 6, Server);
            Int16 receivedToken = 0;
            do
            {
                var receive = await client.ReceiveAsync();
                if (receive.Buffer.Length != 6) continue;
                receivedToken = BitConverter.ToInt16(receive.Buffer, 1);
            } while (receivedToken != token);
            stopwatch.Stop();
        }
    }
}
