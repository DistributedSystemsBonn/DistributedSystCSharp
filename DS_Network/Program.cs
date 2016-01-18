using System;
using System.Threading;
using CookComputing.XmlRpc;
using DS_Network.Election;
using DS_Network.Helpers;
using DS_Network.Network;
using DS_Network.Sync.Ricart;
using DS_Network.Sync.Centralized;

namespace DS_Network
{
    public class Program
    {
        static void Main(string[] args)
        {
            var proxy = XmlRpcProxyGen.Create<IConnectionProxy>();
            var port = NetworkHelper.FindFreePort();
            var ipAddress = NetworkHelper.FindIp().ToString();
            var nodeInfo = new NodeInfo(ipAddress, port);
            var electAlg = new Bully(nodeInfo, proxy);
            var ricartSyncAlgorithm = new RicartSyncAlgorithm(nodeInfo, proxy);
            var centralizedSyncAlgorithm = new CentralizedSyncAlgorithm(nodeInfo, proxy);

            var client = new Node(nodeInfo, proxy, electAlg, ricartSyncAlgorithm.Client, centralizedSyncAlgorithm.Client);
            var server = new Server(port, client, ricartSyncAlgorithm.Server, centralizedSyncAlgorithm.Server);
            var host = new Host(client, server);

            Console.WriteLine("Client IP: " + client.NodeInfo.GetIpAndPort());
            Console.WriteLine("Client ID: " + client.NodeInfo.Id);
            
            while (true)
            {
                Console.Write("["+client.NodeInfo.GetIpAndPort()+"] ");
                Console.WriteLine("The service is ready, please write commands: ");
                var command = Console.ReadLine();
                try
                {
                    client.ProcessCommand(command);
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception.Message);
                }
            }
        }
    }
}
