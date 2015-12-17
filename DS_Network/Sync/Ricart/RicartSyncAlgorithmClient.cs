using System;
using System.Collections.Generic;
using DS_Network.Helpers;
using DS_Network.Network;

namespace DS_Network.Sync.Ricart
{
    public class RicartSyncAlgorithmClient : ISyncAlgorithmClient
    {
        private RicartSyncAlgorithm _module;
        private static readonly Object _outReqToCs = new Object();

        public RicartSyncAlgorithmClient(RicartSyncAlgorithm module)
        {
            _module = module;
        }

        public void SendSyncRequest(IConnectionProxy proxy, List<NodeInfo> toSendHosts)
        {
            //Implementation 1

            //lock (_outReqToCs)
            //{
            //    _module.State = AccessState.Requested;

            //    _module.IsInterested = true;

            //    var timestamp = _module.GetUnixTimestamp();

            //    foreach (var host in toSendHosts)
            //    {
            //        var hostId = host.Id;
            //        proxy.Url = host.GetFullUrl();
            //        proxy.GetSyncRequest(timestamp, hostId);
            //    }

            //    _module.IsInterested = false;
            //}
            _module.State = AccessState.Requested;

            var timestamp = _module.IncrementClock();
            LogHelper.WriteStatus("Client: Current timestamp: " + timestamp);
            _module.IsInterested = true;

            foreach (var host in toSendHosts)
            {
                var hostId = host.Id;
                proxy.Url = host.GetFullUrl();
                LogHelper.WriteStatus("Client: Send request to: " + hostId);
                proxy.GetSyncRequest(timestamp, hostId);
            }

            LogHelper.WriteStatus("Client: Changed status to use resource");
            _module.State = AccessState.Held;
        }

        public void Release()
        {
            _module.State = AccessState.Released;
            _module.IsInterested = false;
            LogHelper.WriteStatus("Client: Released resource");
        }
    }
}
