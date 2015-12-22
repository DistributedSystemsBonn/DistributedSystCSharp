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
        //public ManualResetEvent _hasGotAllMessagesBack = new ManualResetEvent(false);

        public RicartSyncAlgorithmClient(RicartSyncAlgorithm module)
        {
            _module = module;
        }

        public void SendSyncRequestToAllHosts(IConnectionProxy proxy, List<NodeInfo> toSendHosts)
        {
            _module.State = AccessState.Requested;

            var timestamp = _module.IncrementClock();
            LogHelper.WriteStatus("Client: Current timestamp: " + timestamp);
            _module.IsInterested = true;

            List<Task> sendTasks = new List<Task>();

            foreach (var host in toSendHosts)
            {
                var hostId = host.Id;
                proxy.Url = host.GetFullUrl();
                LogHelper.WriteStatus("Client: Send request to: " + hostId);
                sendTasks.Add(Task.Factory.StartNew(() => SendSyncMsg(proxy, timestamp, hostId)));
                //var sendTask = new Thread(() => SendSyncMsg(proxy, timestamp, hostId));
                //sendTask.Start();
            }
            Task.WaitAll(sendTasks.ToArray());

            LogHelper.WriteStatus("Client: Changed status to use resource");
            _module.State = AccessState.Held;
        }

        public void SendSyncMsg(IConnectionProxy proxy, int timestamp, long id)
        {
            if (proxy.GetSyncRequest(timestamp, id))
            {
                
            }
        }

        public void Release(/*IConnectionProxy proxy */)
        {
            _module.State = AccessState.Released;
            _module.IsInterested = false;
            //foreach (var request in _module.Queue)
            //{
            //    //TODO: init url correctly
            //    proxy.Url = NetworkHelper.FormXmlRpcUrl(request.Id);
            //}

            LogHelper.WriteStatus("Client: Released resource");
        }
    }
}
