using System.Net;
using System.Net.Sockets;

namespace DS_Network.Helpers
{
    public static class NetworkHelper
    {
        public static int FindFreePort()
        {
            var l = new TcpListener(IPAddress.Loopback, 0);
            l.Start();
            var port = ((IPEndPoint)l.LocalEndpoint).Port;
            l.Stop();
            return port;
        }
    }
}
