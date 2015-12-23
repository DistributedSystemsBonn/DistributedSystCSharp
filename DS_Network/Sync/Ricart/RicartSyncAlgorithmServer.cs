using System;
using System.Diagnostics;
using System.Threading;
using DS_Network.Helpers;

namespace DS_Network.Sync.Ricart
{
    public class RicartSyncAlgorithmServer : ISyncAlgorithmServer
    {
        private RicartSyncAlgorithm _module;
        //private long _localId;

        public RicartSyncAlgorithmServer(RicartSyncAlgorithm module)
        {
            _module = module;
        }

        /// <summary>
        /// Host receives synchronization request
        /// </summary>
        /// <param name="timestamp"></param>
        /// <param name="id"></param>
        /// <param name="ipAndPort">Ip and port of host who calls this method</param>
        public void GetSyncRequest(int timestamp, long id, string ipAndPort)
        {
            Debug.WriteLine("Server: Start handling at " + _module.LocalNodeInfo.GetIpAndPort() + " request from: " + ipAndPort + " with timestamp: " + timestamp);

            _module.UpdateClock(timestamp);

            // create request object
            var request = new DataRequest()
            {
                Time = timestamp,
                Id = _module.LocalId,
                CallerId = id,
                ipAndPort = ipAndPort
            };

            if (_module.State != AccessState.Held && !_module.IsInterested)
            {
                SendAcceptResponse(ipAndPort);
            }
            else if (_module.State == AccessState.Held)
            {
                _module.AddRequest(request);
            }
            else if (_module.IsInterested)
            {
                if (CompareTime(_module.LamportClock, timestamp, id))
                {
                    //Send accept msg to callee
                    SendAcceptResponse(ipAndPort);
                }
                else
                {
                    _module.AddRequest(request);
                }
            }
        }

        private void SendAcceptResponse(string ipAndPort)
        {
            _module.Proxy.Url = NetworkHelper.FormXmlRpcUrl(ipAndPort);
            //send accept response with parameter which describes our host
            _module.Proxy.GetAcceptResponse(_module.LocalNodeInfo.GetIpAndPort());
        }

        public void GetAcceptResponse(string fromIpAndPort)
        {
            var myIp = _module.LocalNodeInfo.GetIpAndPort();
            Debug.WriteLine("Start removing from " + myIp + " .Key with ip: " + fromIpAndPort);
            if (_module.AcceptList.Exists(x => x == fromIpAndPort))
            {
                _module.AcceptList.Remove(fromIpAndPort);
                Debug.WriteLine("List count from " + myIp + " = " + _module.AcceptList.Count);
                //TODO: error handle. need to check
                //return;
                //throw new ArgumentException("Element in accept list doesnt exist: " + fromIpAndPort);
            }
            else
            {
                throw new ArgumentException("Element in accept list doesnt exist: " + fromIpAndPort);
            }

            //_module.AcceptList.Remove(fromIpAndPort);

            //check if all accept messages received. if yes, start accessing to resource
            if (_module.AcceptList.Count == 0)
            {
                Debug.WriteLine("Reset event has got messages at host: " + myIp);
                //TODO: ManualResetEvent _isAcceptMessagesFinished.Set() in Node
                _module.Client.HasGotAllMessagesBack.Set();
            }
        }

        public bool CompareTime(int ownLamportClock, int requestLamportClock, long remoteId)
        {
            if (requestLamportClock < ownLamportClock)
                return true;
            if (requestLamportClock > ownLamportClock)
                return false;
            return remoteId < _module.LocalId;
        }
    }
}
