using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Text.RegularExpressions;
using CookComputing.XmlRpc;
using DS_Network.Election;
using DS_Network.Helpers;
using DS_Network.Network;
using DS_Network.Sync.Ricart;

namespace DS_Network
{
    class Program
    {
        static void Main(string[] args)
        {
            var proxy = XmlRpcProxyGen.Create<IConnectionProxy>();
            var port = NetworkHelper.FindFreePort();
            var electAlg = new Bully();
            var ipAddress = NetworkHelper.FindIp().ToString();
            var nodeInfo = new NodeInfo(ipAddress, port);
            var syncAlgorithm = new RicartSyncAlgorithm(nodeInfo, proxy);

            var client = new Node(nodeInfo, proxy, electAlg, syncAlgorithm.Client, port); //client
            var server = new Server(port, syncAlgorithm.Server, client);
            server.Run();

            Console.WriteLine("Client IP: " + client.NodeInfo.GetIpAndPort());
            Console.WriteLine("Client ID: " + client.NodeInfo.Id);
            
            while (true)
            {
                
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
