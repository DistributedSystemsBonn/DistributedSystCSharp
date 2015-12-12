using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Http;
using System.Text;
using System.Threading.Tasks;
using CookComputing.XmlRpc;

namespace DS_Network.Network
{
    public class Server : MarshalByRefObject, IConnectionService
    {
        private int _port;
        private static Node _client;

        public Server()
        {
            
        }

        public Server(int port, Node client)
        {
            _port = port;
            _client = client;
        }

        public bool join(string ipAndPort)
        {
            return true;
        }

        public bool signOff()
        {
            throw new NotImplementedException();
        }

        public bool start()
        {
            throw new NotImplementedException();
        }


        //public Object[] getHosts()
        public Object[] getHosts(String ipAndPortCallee)
        {
            var hostList = _client.HostLookup;
            var listToSend = new ArrayList();

            foreach (var host in hostList.Values)
            {
                listToSend.Add(host.GetIpAndPort());
            }

            _client.AddNewComputer(ipAndPortCallee);

            return listToSend.ToArray();
        }

        public void Run()
        {
            IDictionary props = new Hashtable();
            props["name"] = "MyHttpChannel";
            props["port"] = _port;
            var channel = new HttpChannel(
              props,
              null,
              new XmlRpcServerFormatterSinkProvider()
           );
            ChannelServices.RegisterChannel(channel, false);

            RemotingConfiguration.RegisterWellKnownServiceType(
              typeof(Server),
              "xmlrpc",
              WellKnownObjectMode.Singleton);
        }
    }
}
