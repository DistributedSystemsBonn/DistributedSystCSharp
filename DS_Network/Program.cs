using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Text.RegularExpressions;
using DS_Network.Helpers;
using DS_Network.Network;
using Microsoft.Samples.XmlRpc;

namespace DS_Network
{
    class Program
    {
        static void Main(string[] args)
        {
            ////TODO: 
            //ServiceReference1.Service1Client client = new ServiceReference1.Service1Client();
            //TODO: start WCF Service

            ChannelFactory<IConnectionService> cf = new ChannelFactory<IConnectionService>(
                new WSHttpBinding(), "http://www.example.com/xmlrpc");

            cf.Endpoint.Behaviors.Add(new XmlRpcEndpointBehavior());

            var client = cf.CreateChannel();

            Node newNode = new Node(client); //client
            while (true)
            {
                Console.WriteLine("The service is ready, please write commands: ");
                var command = Console.ReadLine();
                try
                {
                    newNode.ProcessCommand(command);
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
