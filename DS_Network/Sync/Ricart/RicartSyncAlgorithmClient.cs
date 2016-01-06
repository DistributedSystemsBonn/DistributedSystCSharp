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
    public class RicartSyncAlgorithmClient : ISyncAlgorithmClient
    {
        private RicartSyncAlgorithm _module;
        //lock until all messages received
        public ManualResetEvent HasGotAllMessagesBack = new ManualResetEvent(false);
        
        public RicartSyncAlgorithmClient(RicartSyncAlgorithm module)
        {
            _module = module;
        }

        public void SendSyncRequestToAllHosts(List<NodeInfo> toSendHosts)
        {
            lock (Shared.SharedLock)
            {
                _module.State = AccessState.Requested;

                LogHelper.WriteStatus("Client: " + _module.LocalId + ": Current timestamp: " + _module.Clock.Value);

                LogHelper.WriteStatus("Client: " + _module.LocalId + ": Capacity: " + toSendHosts.Count);
                _module.IsInterested = true;

                foreach (var host in toSendHosts)
                {
                    var newThread =
                        new Thread(
                            () => SendSyncMsg(_module.Proxy, host));
                    newThread.Start();
                    Thread.Sleep(5);
                }
            }
            

            //wait until receive all messages. .Set() method is called in RicartSyncAlgServer
            HasGotAllMessagesBack.WaitOne();

            LogHelper.WriteStatus("CLIENT: RECV ALL ACCEPT MESSAGES AT: " + _module.LocalId);
            _module.State = AccessState.Held;
        }

        public void SendSyncMsg(IConnectionProxy proxy, NodeInfo toNode)
        {
            lock (Shared.SharedLock)
            {
                var logicClockTs = _module.Clock.SendEventHandle();

                _module.AddToAcceptList(toNode.GetIpAndPort());
                var urlToSend = toNode.GetFullUrl(); ;
                proxy.Url = urlToSend;
                Debug.WriteLine("CLIENT: SEND REQ FROM: " + _module.LocalNodeInfo.GetIpAndPort() + " TO: " + _module.Proxy.Url);
                _module.Proxy.GetSyncRequest(logicClockTs, _module.LocalNodeInfo.Id, _module.LocalNodeInfo.GetIpAndPort());
            }
        }

        public void Release()
        {
            _module.State = AccessState.Released;
            _module.IsInterested = false;

            foreach (var request in _module.Queue)
            {
                _module.Server.SendAcceptResponse(request.ipAndPort);
            }

            _module.Queue.Clear();

            LogHelper.WriteStatus("Client: Released resource at " + _module.LocalNodeInfo.GetIpAndPort());
        }
    }
}
