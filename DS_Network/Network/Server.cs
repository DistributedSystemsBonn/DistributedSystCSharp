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
using DS_Network.Sync;

namespace DS_Network.Network
{
    public class Server : MarshalByRefObject, IConnectionService, ISyncAlgorithmServer
    {
        private int _port;
        private static Node _client;
        private static ISyncAlgorithmServer _syncAlgorithmServer;

        public Server()
        {
            
        }

        public Server(int port, ISyncAlgorithmServer syncAlg, Node client)
        {
            _port = port;
            _client = client;
            _syncAlgorithmServer = syncAlg;
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
            props["name"] = "MyHttpChannel" + ChannelServices.RegisteredChannels.Length;
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
            Console.WriteLine("Set Master Node: " + ipAndPortMaster);
            _client.SetMasterNode(ipAndPortMaster, true);
        }

        /// <summary>
        /// Read string variable from Master Node or Node (not used)
        /// </summary>
        /// <returns></returns>
        public string readResource(string ipAndPort)
        {
            Console.WriteLine("Read resource from: " + ipAndPort);
            return _client.Resource;
        }

        public void updateResource(string updateStr, string ipAndPort)
        {
            Console.WriteLine("Update resource from: " + ipAndPort);
            _client.Resource = updateStr;
            //_module.AdjustLastRequestTsToNow();
        }

        public void GetSyncRequest(int timestamp, long id, string ipAndPort)
        {
            _syncAlgorithmServer.GetSyncRequest(timestamp, id, ipAndPort);
        }

        public void GetAcceptResponse(string fromIpAndPort, int timestamp)
        {
            _syncAlgorithmServer.GetAcceptResponse(fromIpAndPort, timestamp);
        }
    }
}
