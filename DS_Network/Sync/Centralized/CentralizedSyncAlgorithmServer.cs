using DS_Network.Helpers;

namespace DS_Network.Sync.Centralized
{
    public class CentralizedSyncAlgorithmServer : ICentralizedSyncAlgorithmServer
    {
        private CentralizedSyncAlgorithm _module;
        public CentralizedSyncAlgorithmServer(CentralizedSyncAlgorithm module)
        {
            _module = module;
        }

        /// <summary>
        /// Master: Get Sync Request msg from hosts
        /// </summary>
        public void GetSyncRequest_CT(string id, string ipAndPort) 
        {
            var request = new DataRequest()
            {
                Time = 0,
                Id = 0,
                CallerId = long.Parse(id),
                IpAndPort = ipAndPort
            };

            LogHelper.WriteStatus("Master: get request from " + id);

            if (_module.State == AccessState.Released)
            { // if resource is released 
                SendAcceptResponse(ipAndPort);
            }
            else if (_module.State == AccessState.Held)
            { // if resource is held by a host, add the request into queue
                _module.AddRequest(request);
            }
        }

        /// <summary>
        /// Master: Send msg to a host - accept for resource
        /// </summary>
        private void SendAcceptResponse(string ipAndPort)
        {
            LogHelper.WriteStatus("Accept for Resource to " + ipAndPort);
            _module.Proxy.Url = NetworkHelper.FormXmlRpcUrl(ipAndPort);
            //send accept response
            _module.State = AccessState.Held;
            _module.Proxy.GetAcceptResponse_CT();
        }

        /// <summary>
        /// Get Accept message from master
        /// </summary>
        public void GetAcceptResponse_CT() 
        {
            LogHelper.WriteStatus("Accepted for resource");
            _module.Client.IsAllowed.Set();
        }

        /// <summary>
        /// Master: Get released msg from a host
        /// </summary>
        public void GetReleasedMsg_CT(string id, string fromIpAndPort)
        {
            LogHelper.WriteStatus("Master: Released from " + fromIpAndPort);
            _module.State = AccessState.Released;

            var next = _module.PopRequest();
            if (next != null)
            {
                SendAcceptResponse(next.IpAndPort);
            }
        }
    }
}
