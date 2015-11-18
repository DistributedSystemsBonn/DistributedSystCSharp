using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DS_Network.Network;

namespace DS_Network
{
    class Program
    {
        static void Main(string[] args)
        {
            //TODO: 
            ServiceReference1.Service1Client client = new ServiceReference1.Service1Client();
            //TODO: start WCF Service
            Node newNode = new Node(client);

            client.Close();

        }
    }
}
