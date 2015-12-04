using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace DS_Network.Network
{
    public class Node
    {
        private Dictionary<int, String> _hostLookup = new Dictionary<int, string>();
        private IPAddress _address;

        //TODO: put WCF service to constructor as parameter and use it in methods (like join...)
        public Node() //ServiceReference1.Service1Client client
        {
            IPHostEntry host;
            host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    _address = ip;
                }
            }
            
            if (_address == null)
            {
                throw new Exception("Cannot find proper ip address");
            }
        }

        public void Join(String address)
        {
            //TODO: join. send message to just one machine. And then it propagates the message
        }

        public void SignOff()
        {
            //TODO: signoff message (we need to ask, is this a broadcast message or one-node message)???
        }

        public void Start()
        {
            //TODO: 1. send message to all other nodes
            //IN LOOP for 20 seconds
            //2. wait random amount of time
            //3. read string variable from master node
            //4. append some random english word to this string
            //5. write updated string to master node
            //END LOOP

            //6. Node fetches from Master node the final string
            //7. And writes this final string on screen

            //NOTE: read and write operations should be syncronized
        }

    }
}
