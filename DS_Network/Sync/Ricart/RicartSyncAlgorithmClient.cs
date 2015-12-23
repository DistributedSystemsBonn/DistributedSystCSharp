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
            _module.State = AccessState.Requested;

            var logicClockTs = _module.IncrementLamportClock();
            LogHelper.WriteStatus("Client: Current timestamp: " + logicClockTs);
            _module.IsInterested = true;

            foreach (var host in toSendHosts)
            {
                var newThread =
                    new Thread(
                        () => SendSyncMsg(_module.Proxy, host, logicClockTs));
                Debug.WriteLine("Client: Send request from: " + _module.LocalNodeInfo.GetIpAndPort() + " to: " + _module.Proxy.Url);
                _module.Proxy.Url = host.GetFullUrl();
                newThread.Start();
                _module.Proxy.Url = host.GetFullUrl();
                Thread.Sleep(15);
            }

            //wait until receive all messages. .Set() method is called in RicartSyncAlgServer
            HasGotAllMessagesBack.WaitOne();

            LogHelper.WriteStatus("Client: Received all ACCEPT MESSAGES AT: " + _module.LocalId);
            _module.State = AccessState.Held;
        }

        public void SendSyncMsg(IConnectionProxy proxy, NodeInfo toNode, int logicClockTs)
        {
            _module.AcceptList.Add(toNode.GetIpAndPort());
            proxy.Url = toNode.GetFullUrl();
            _module.Proxy.GetSyncRequest(logicClockTs, toNode.Id, _module.LocalNodeInfo.GetIpAndPort());
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
