using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DS_Network.Helpers;
using DS_Network.Network;

namespace DS_Network.Sync.Ricart
{
    public class RicartSyncAlgorithmClient : ISyncAlgorithmClient
    {
        private RicartSyncAlgorithm _module;
        public ManualResetEvent HasGotAllMessagesBack = new ManualResetEvent(false);

        public RicartSyncAlgorithmClient(RicartSyncAlgorithm module)
        {
            _module = module;
        }

        public void SendSyncRequestToAllHosts(List<NodeInfo> toSendHosts)
        {
            _module.State = AccessState.Requested;

            var logicClockTs = _module.IncrementLamportClock();
            //var timestamp = _module.IncrementClock();
            LogHelper.WriteStatus("Client: Current timestamp: " + logicClockTs);
            _module.IsInterested = true;

            //var sendTasks = new List<Task>();
            var newThreads = new List<Thread>();

            foreach (var host in toSendHosts)
            {
                var hostId = host.Id;

                _module.Proxy.Url = host.GetFullUrl();
                LogHelper.WriteStatus("Client: Send request to: " + hostId);
                _module.AcceptList.Add(host.GetIpAndPort());
                newThreads.Add(new Thread(() => _module.Proxy.GetSyncRequest(logicClockTs, hostId, _module.LocalNodeInfo.GetIpAndPort())));
                //newTask.Start();
                //sendTasks.Add(Task.Factory.StartNew(() => _module.Proxy.GetSyncRequest(logicClockTs, hostId)));
            }

            foreach (var newThread in newThreads)
            {
                newThread.Start();
            }
            //wait until receive all messages. .Set() method is called in RicartSyncAlgServer
            HasGotAllMessagesBack.WaitOne();

            //Task.WaitAll(sendTasks.ToArray());

            LogHelper.WriteStatus("Client: Received all ACCEPT MESSAGES AT: " + _module.LocalId);
            _module.State = AccessState.Held;
        }

        public void Release()
        {
            _module.State = AccessState.Released;
            _module.IsInterested = false;

            foreach (var request in _module.Queue)
            {
                //TODO: init url correctly
                _module.Proxy.Url = NetworkHelper.FormXmlRpcUrl(request.ipAndPort);
                _module.Proxy.GetAcceptResponse(_module.LocalNodeInfo.GetIpAndPort());
            }

            _module.Queue.Clear();

            LogHelper.WriteStatus("Client: Released resource");
        }
    }
}
