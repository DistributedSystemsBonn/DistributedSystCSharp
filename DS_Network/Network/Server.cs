using System;
using System.Collections;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Http;
using System.Threading;
using CookComputing.XmlRpc;
using DS_Network.Helpers;
using DS_Network.Sync;

namespace DS_Network.Network
{
    public class Server : MarshalByRefObject, IConnectionService, IRicartSyncAlgorithmServer, ICentralizedSyncAlgorithmServer
    {
        private int _port;
        private static Node _client;
        private static IRicartSyncAlgorithmServer _ricartSyncAlgServer;
        private static ICentralizedSyncAlgorithmServer _centralizedSyncAlgServer;

        public Server()
        {
            
        }

        public Server(int port, Node client,
            IRicartSyncAlgorithmServer ricartSyncAlgorithm, ICentralizedSyncAlgorithmServer centralizedSyncAlgorithm)
        {
            _port = port;
            _client = client;
            _ricartSyncAlgServer = ricartSyncAlgorithm;
            _centralizedSyncAlgServer = centralizedSyncAlgorithm;
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

        #region SignOff operation
        /// <summary>
        /// Sign off from network.
        /// </summary>
        /// <returns></returns>
        public bool SignOff(string ipAndPort)
        {
            var hostList = _client.HostLookup;

            LogHelper.WriteStatus("Node [" + ipAndPort + "] is signed off.");
            return hostList.Remove(ipAndPort);
        }
        #endregion

        #region Join operation
        /// <summary>
        /// Get list of hosts
        /// </summary>
        /// <param name="ipAndPortCallee"></param>
        /// <returns></returns>
        public Object[] GetHosts(string ipAndPortCallee)
        {
            var hostList = _client.HostLookup;
            var listToSend = new ArrayList();

            foreach (var host in hostList.Values)
            {
                listToSend.Add(host.GetIpAndPort());
            }

            AddNewHost(ipAndPortCallee);

            return listToSend.ToArray();
        }

        /// <summary>
        /// Add new host to client (when join operation)
        /// </summary>
        public void AddNewHost(string ipAndPort)
        {
            LogHelper.WriteStatus("New node joined: " + ipAndPort);
            _client.AddNewHost(ipAndPort);
        }
        #endregion

        #region start and sync
        /// <summary>
        /// Start algorithm - every node receive this message.
        /// </summary>
        public void GetStartMsg(bool isRicartAlgorithm)
        {
            Thread proc = new Thread(() => _client.StartProcess(isRicartAlgorithm));
            proc.Start();
        }

        public void GetSyncRequest_CT(long id, string ipAndPort)
        {
            _centralizedSyncAlgServer.GetSyncRequest_CT(id, ipAndPort);
        }

        public void GetReleasedMsg_CT(long id, string fromIpAndPort)
        {
            _centralizedSyncAlgServer.GetReleasedMsg_CT(id, fromIpAndPort);
        }

        public void GetAcceptResponse_CT()
        {
            _centralizedSyncAlgServer.GetAcceptResponse_CT();
        }

        public void GetSyncRequest_RA(int timestamp, long id, string ipAndPort)
        {
            _ricartSyncAlgServer.GetSyncRequest_RA(timestamp, id, ipAndPort);
        }

        public void GetAcceptResponse_RA(string fromIpAndPort, int timestamp)
        {
            _ricartSyncAlgServer.GetAcceptResponse_RA(fromIpAndPort, timestamp);
        }

        #endregion

        #region Election - Bully Algorithm

        /// <summary>
        /// Receiving election message from other candidate host
        /// </summary>
        public bool ReceiveElectionMsg(string id)
        {
            Thread election = new Thread(() => _client.ElectMasterNodeByReceivingMsg(id));
            election.Start();

            return true;    // always return true;
        }

        /// <summary>
        /// Setting master node by receiving message from the elected master node
        /// </summary>
        /// <param name="ipAndPortMaster"></param>
        public void SetMasterNode(string ipAndPortMaster)
        {
            //Console.WriteLine("Set Master Node: " + ipAndPortMaster);
            _client.SetMasterNode(ipAndPortMaster);
        }

        #endregion

        #region Read and Write
        /// <summary>
        /// Read string variable from Master Node or Node (not used)
        /// </summary>
        /// <returns></returns>
        public string ReadResource(string ipAndPort)
        {
            Console.WriteLine("\nRead resource from: " + ipAndPort);
            Console.WriteLine("::Current String:: " + _client.Resource);
            return _client.Resource;
        }

        public void UpdateResource(string updateStr, string ipAndPort)
        {
            Console.WriteLine("\nUpdate resource from: " + ipAndPort);
            _client.Resource = updateStr;
            Console.WriteLine("::String:: " + _client.Resource);
            //_module.AdjustLastRequestTsToNow();
        }
        #endregion
    }
}
