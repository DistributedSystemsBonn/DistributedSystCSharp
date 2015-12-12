using System;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
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

        public static IPAddress FindIp()
        {
            IPHostEntry host;
            IPAddress address = null;

            var networkList = NetworkInterface.GetAllNetworkInterfaces();
            var local = networkList.FirstOrDefault(i => i.Description == "LogMeIn Hamachi Virtual Ethernet Adapter");

            if (local == null)
            {
                host = Dns.GetHostEntry(Dns.GetHostName());
                foreach (IPAddress ip in host.AddressList)
                {
                    if (ip.AddressFamily == AddressFamily.InterNetwork)
                    {
                        address = ip;
                    }
                }
            }
            else
            {
                var props = local.GetIPProperties();
                address = props.UnicastAddresses[0].Address;
            }

            if (address == null)
            {
                throw new Exception("Cannot find proper ip address");
            }

            return address.MapToIPv4();
        }
    }
}
