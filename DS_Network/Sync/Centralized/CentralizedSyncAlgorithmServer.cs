using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DS_Network.Helpers;

namespace DS_Network.Sync.Centralized
{
    public class CentralizedSyncAlgorithmServer : ISyncAlgorithmServer
    {
        private CentralizedSyncAlgorithm _module;
        public CentralizedSyncAlgorithmServer(CentralizedSyncAlgorithm module)
        {
            _module = module;
        }

        public void GetSyncRequest(int timestamp, long id, string ipAndPort) 
        {
            var request = new DataRequest()
            {
                Time = timestamp,
                Id = _module.LocalId,
                CallerId = id,
                ipAndPort = ipAndPort
            };

            
            if (_module.State == AccessState.Released)
            { // if resource is released 
                SendAcceptResponse(ipAndPort);
            }
            else if (_module.State == AccessState.Held)
            { // if resource is held by some client, add the request into queue
                _module.AddRequest(request);
            }
        }

        private void SendAcceptResponse(string ipAndPort)
        {
            _module.Proxy.Url = NetworkHelper.FormXmlRpcUrl(ipAndPort);
            //send accept response
            _module.Proxy.GetAcceptResponse("");
        }

        public void GetAcceptResponse(string fromIpAndPort) 
        {
            _module.Client._isAllowed.Set();
        }

        public void GetReleasedMsg(string fromIpAndPort)
        {
            _module.State = AccessState.Released;

            var next = _module.PopRequest();
            if (next != null)
            {
                SendAcceptResponse(next.ipAndPort);
            }
        }
    }
}
