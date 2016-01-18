using System.Diagnostics;
using System.Threading;
using DS_Network.Helpers;
using DS_Network.Network;

namespace DS_Network.Sync.Ricart
{
    /// <summary>
    /// Server part of ricart algorithm
    /// </summary>
    public class RicartSyncAlgorithmServer : IRicartSyncAlgorithmServer
    {
        private RicartSyncAlgorithm _module;
        
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
        public void GetSyncRequest_RA(int timestamp, long id, string ipAndPort)
        {
            LogHelper.WriteStatus("SERVER: RECV " + _module.LocalNodeInfo.GetIpAndPort() + " FROM: " + ipAndPort + " TIME: " + timestamp);
            // create request object
            var request = new DataRequest()
            {
                Time = timestamp,
                Id = _module.LocalId,
                CallerId = id,
                IpAndPort = ipAndPort
            };

            if (_module.State != AccessState.Held && !_module.IsInterested)
            {
                //Send accept msg to callee
                SendAcceptResponse(ipAndPort);
            }
            else if (_module.State == AccessState.Held)
            {
                _module.AddRequest(request);
            }
            else if (_module.IsInterested)
            {
                if (_module.Clock.CompareTime(timestamp, id))
                {   // request timestamp is smaller than this node's timestamp.
                    //Send accept msg to callee
                    SendAcceptResponse(ipAndPort);
                }
                else
                {
                    _module.AddRequest(request);
                }
            }
            _module.Clock.ReceiveEventHandle(timestamp);
        }
        /// <summary>
        /// send accept response to a host
        /// </summary>
        /// <param name="ipAndPort"></param>
        public void SendAcceptResponse(string ipAndPort)
        {
            //Send event in clock
            _module.Clock.SendEventHandle();
            var myIp = _module.LocalNodeInfo.GetIpAndPort();
            LogHelper.WriteStatus("SERVER: " + myIp + " SEND OK TO: " + ipAndPort);
            //Debug.WriteLine("SERVER: " + myIp + "SEND OK TO: " + ipAndPort);

            lock (Shared.SendLock)
            {
                _module.Proxy.Url = NetworkHelper.FormXmlRpcUrl(ipAndPort);
                //send accept response with parameter which describes our host
                _module.Proxy.GetAcceptResponse_RA(_module.LocalNodeInfo.GetIpAndPort(), _module.Clock.Value);
                //var newSendThread =
                //    new Thread(
                //        () =>
                //            _module.Proxy.GetAcceptResponse_RA(_module.LocalNodeInfo.GetIpAndPort(), _module.Clock.Value));
                //newSendThread.Start();
            }
        }
        /// <summary>
        /// get accept response from a host
        /// </summary>
        /// <param name="fromIpAndPort"></param>
        /// <param name="timestamp"></param>
        public void GetAcceptResponse_RA(string fromIpAndPort, int timestamp)
        {
            var myIp = _module.LocalNodeInfo.GetIpAndPort();
            
            _module.RemoveFromAcceptList(fromIpAndPort);
            
            //check if all accept messages received. if yes, start accessing to resource
            if (_module.IsGotAllOk())
            {
                LogHelper.WriteStatus("RESET. GOT ALL AT: " + myIp);
                //Debug.WriteLine("RESET. GOT ALL AT: " + myIp);
                //ManualResetEvent _isAcceptMessagesFinished.Set() in Node
                _module.Client.HasGotAllMessagesBack.Set();
            }

            //Clock: recv handle
            _module.Clock.ReceiveEventHandle(timestamp);
        }
    }
}
