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

            Node client = new Node(proxy, port); //client
            Server server = new Server(port);
            server.Run();
            //client.Run();

            Console.WriteLine(client.NodeInfo.GetIpAndPort());
            while (true)
            {
                
                Console.WriteLine("The service is ready, please write commands: ");
                var command = Console.ReadLine();
                try
                {
                    client.ProcessCommand(command);
                }
                catch (ArgumentException exception)
                {
                    Console.WriteLine(exception.Message);
                }
                
            }
            

            //client.Close();

        }
    }
}
