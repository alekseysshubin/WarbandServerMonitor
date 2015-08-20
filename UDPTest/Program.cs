using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace UDPTest
{
    public class Program
    {
        static void Main(string[] args)
        {
            Task.Run(async () => await Ping());
            Console.ReadKey();
            _stop = true;
        }

        private void Test()
        {
            var client = new UdpClient(52327);
            //client.Send(new byte[6] { 0x06, 0xe0, 0xe4, 0x00, 0x00, 0x01 }, 6, new IPEndPoint(IPAddress.Parse("109.120.190.99"), 7240));
            //client.Send(new byte[6] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x01 }, 6, new IPEndPoint(IPAddress.Parse("109.120.190.99"), 7240));
            client.Send(new byte[6] { 0x06, 0x00, 0x00, 0x00, 0x00, 0x01 }, 6, new IPEndPoint(IPAddress.Parse("109.120.190.99"), 7240));
            IPEndPoint remoteEp = null;
            var bytes = client.Receive(ref remoteEp);
        }

        private static bool _stop;
        private static Stopwatch _stopwatch = new Stopwatch();

        private async static Task Ping()
        {
            while (!_stop)
            {
                var client = new UdpClient(52327);
                _stopwatch.Restart();
                await client.SendAsync(new byte[] { 0x06, 0x00, 0x00, 0x00, 0x00, 0x01 }, 6, new IPEndPoint(IPAddress.Parse("109.120.190.99"), 7240));
                await client.ReceiveAsync();
                _stopwatch.Stop();
                Console.Write(_stopwatch.ElapsedMilliseconds + " ");
                client.Close();
                await Task.Delay(5000);
            }
        }
    }
}