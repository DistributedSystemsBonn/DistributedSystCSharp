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

        public Server(int port)
        {
            _port = port;
        }

        public bool join(string ipAndPort)
        {
            throw new NotImplementedException();
        }

        public bool signOff()
        {
            throw new NotImplementedException();
        }

        public bool start()
        {
            throw new NotImplementedException();
        }

        public Object[] getHosts()
        {
            return new Object[]
            {
                "111", "222"
            };
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
