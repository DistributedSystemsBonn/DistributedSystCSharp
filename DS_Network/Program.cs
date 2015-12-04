using System.ServiceModel;
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



            //client.Close();

        }
    }
}
