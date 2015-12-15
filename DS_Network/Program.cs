using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Text.RegularExpressions;
using CookComputing.XmlRpc;
using DS_Network.Helpers;
using DS_Network.Network;

namespace DS_Network
{
    class Program
    {
        static void Main(string[] args)
        {
            var proxy = XmlRpcProxyGen.Create<IConnectionProxy>();
            int port = NetworkHelper.FindFreePort();

            var client = new Node(proxy, port); //client
            var server = new Server(port, client);
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
