using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using DS_Network.Helpers;
using DS_Network.Network;

namespace DS_Network.Sync.Ricart
{
    /// <summary>
    /// Client part of ricart algorithm
    /// </summary>
    public class RicartSyncAlgorithmClient : IRicartSyncAlgorithmClient
    {
        private RicartSyncAlgorithm _module;
        //lock until all messages received
        public ManualResetEvent HasGotAllMessagesBack = new ManualResetEvent(false);
        
        public RicartSyncAlgorithmClient(RicartSyncAlgorithm module)
        {
            _module = module;
        }

        /// <summary>
        /// Send Request msg to all the hosts in the network
        /// </summary>
        /// <param name="toSendHosts">list without Master Node and itself</param>
        public void SendSyncRequestToAllHosts_RA(List<NodeInfo> toSendHosts)
        {
            //wait until receive all messages.
            HasGotAllMessagesBack.Reset();
            _module.State = AccessState.Requested;
            _module.IsInterested = true;
            LogHelper.WriteStatus("Client: [" + _module.LocalId + "] Current timestamp: " + _module.Clock.Value);
            LogHelper.WriteStatus("Client: [" + _module.LocalId + "] Capacity: " + toSendHosts.Count);

            //Thread.Sleep(1000);

            foreach (var host in toSendHosts)
            {
                var host1 = host;
                LogHelper.WriteStatus(" CLIENT: SEND REQ FROM: [" + _module.LocalNodeInfo.GetIpAndPort() + "] TO: [" +
                                      host1.Id + "]");
                var newThread =
                    new Thread(
                        () => SendSyncMsg(_module.Proxy, host1));
                newThread.Start();
            }

            //wait until receive all messages. .Set() method is called in RicartSyncAlgServer
            HasGotAllMessagesBack.WaitOne();

            LogHelper.WriteStatus("CLIENT: RECV ALL ACCEPT MESSAGES AT: [" + _module.LocalId + "]");
            _module.State = AccessState.Held;
        }

        /// <summary>
        /// Send Req message to a node
        /// </summary>
        private void SendSyncMsg(IConnectionProxy proxy1, NodeInfo toNode)
        {
            var logicClockTs = _module.Clock.SendEventHandle();

            _module.AddToAcceptList(toNode.GetIpAndPort());
            var urlToSend = toNode.GetFullUrl();
                
            //LogHelper.WriteStatus("CLIENT: SEND REQ FROM: [" + _module1.LocalNodeInfo.GetIpAndPort() + "] TO: [" +
            //                      proxy1.Url + "]");
            //.WriteLine("CLIENT: SEND REQ FROM: " + _module1.LocalNodeInfo.GetIpAndPort() + " TO: " + _module1.Proxy1.Url);
            lock (Shared.SendLock)
            {
                proxy1.Url = urlToSend;
                proxy1.GetSyncRequest_RA(logicClockTs.ToString(), _module.LocalNodeInfo.Id.ToString(), _module.LocalNodeInfo.GetIpAndPort());
            }
        }

        /// <summary>
        /// Release the resource.
        /// Send Accept responses to all nodes in the queue
        /// </summary>
        public void Release_RA()
        {
            _module.PrintQueue();

            _module.State = AccessState.Released;
            _module.IsInterested = false;

            foreach (var request in _module.Queue)
            {
                _module.Server.SendAcceptResponse(request.IpAndPort);
            }

            _module.Queue.Clear();

            LogHelper.WriteStatus("Client: Released resource at [" + _module.LocalNodeInfo.GetIpAndPort() + "]");
        }

        /// <summary>
        /// Reset variables to start again
        /// </summary>
        public void RicartReset()
        {
            _module.RicartReset();
        }
    }
}
