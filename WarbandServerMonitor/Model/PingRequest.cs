using System;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;

namespace WarbandServerMonitor.Model
{
    public class PingRequest
    {
        public IPEndPoint EndPoint { get; set; }
        public TaskCompletionSource<long> TaskCompletionSource { get; set; }
        public Stopwatch Stopwatch { get; set; }
        public Int16 Token { get; set; }
    }
}
