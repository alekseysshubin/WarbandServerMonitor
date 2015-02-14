using System.Collections.Concurrent;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace WarbandServerMonitor.Model
{
    public class Pinger
    {
        // There is no ConcurrentCollection or ConcurrentList in .NET, so have to use ConcurrentDictionary.
        private readonly ConcurrentDictionary<PingRequest, PingRequest> _requests;

        private readonly UdpClient _client;
        private bool _stop;

        public Pinger()
        {
            _client = new UdpClient();
            _requests = new ConcurrentDictionary<PingRequest, PingRequest>();
            Receiver();
        }

        public Task<long> Ping(IPAddress ip, int port)
        {
            var request = new PingRequest
            {
                EndPoint = new IPEndPoint(ip, port),
                TaskCompletionSource = new TaskCompletionSource<long>()
            };
            _requests.TryAdd(request, request);
            return request.TaskCompletionSource.Task;
        }

        private async void Receiver()
        {
            while (true)
            {
                if (_stop) return;

                var response = await _client.ReceiveAsync();
                if (response.Buffer.Length != 6) continue;
                var token = (response.Buffer[2] << 8) + response.Buffer[1];
                var request = _requests.FirstOrDefault(x => x.Value.Token == token).Value;
                if (request == null) continue;

                request.Stopwatch.Stop();
                _requests.TryRemove(request, out request);
                request.TaskCompletionSource.SetResult(request.Stopwatch.ElapsedMilliseconds);
            }
        }

        private void SendPings()
        {
            while (!_requests.IsEmpty)
            {
                
            }
        }
    }
}
