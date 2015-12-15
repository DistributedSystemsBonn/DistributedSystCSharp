using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Http;
using System.Text;
using System.Threading;
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

        /// <summary>
        /// Sign off from network.
        /// </summary>
        /// <returns></returns>
        public bool signOff(string ipAndPort)
        {
            var hostList = _client.HostLookup;

            return hostList.Remove(ipAndPort);
        }

        /// <summary>
        /// Start algorithm
        /// </summary>
        /// <returns></returns>
        public bool start()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get list of hosts
        /// </summary>
        /// <param name="ipAndPortCallee"></param>
        /// <returns></returns>
        public Object[] getHosts(String ipAndPortCallee)
        {
            var hostList = _client.HostLookup;
            var listToSend = new ArrayList();

            foreach (var host in hostList.Values)
            {
                listToSend.Add(host.GetIpAndPort());
            }

            _client.AddNewHost(ipAndPortCallee);

            return listToSend.ToArray();
        }

        /// <summary>
        /// Add new host to client (when join operation)
        /// </summary>
        /// <param name="ipAndPort"></param>
        public void addNewHost(string ipAndPort)
        {
            _client.AddNewHost(ipAndPort);
        }

        /// <summary>
        /// Run server
        /// </summary>
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
              //"/",
              WellKnownObjectMode.Singleton);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool ReceiveElectionMsg(string id)
        {
            Thread election = new Thread(() => _client.ElectMasterNodeByReceivingMsg(id));
            election.Start();

            return true;    // always return true;
        }

        public void SetMasterNode(String ipAndPortMaster)
        {
            _client.SetMasterNode(ipAndPortMaster);
        }
    }
}
