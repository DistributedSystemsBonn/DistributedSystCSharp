using System;
using System.Diagnostics;
using System.Threading;
using DS_Network.Helpers;

namespace DS_Network.Sync.Ricart
{
    /// <summary>
    /// Server part of ricart algorithm
    /// </summary>
    public class RicartSyncAlgorithmServer : ISyncAlgorithmServer
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
        public void GetSyncRequest(int timestamp, long id, string ipAndPort)
        {
            //lock (Shared.SharedLock)
            //{
            _module.PrintQueue();
                Debug.WriteLine("SERVER: RECV " + _module.LocalNodeInfo.GetIpAndPort() + " FROM: " + ipAndPort + " TIME: " + timestamp);
                //SendAcceptResponse(ipAndPort);
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
                    if (_module.Clock.CompareTime(timestamp, id))
                    {
                        //Send accept msg to callee
                        SendAcceptResponse(ipAndPort);
                    }
                    else
                    {
                        _module.AddRequest(request);
                    }
                }
                //else
                //{
                //    SendAcceptResponse(ipAndPort);
                //}
                _module.Clock.ReceiveEventHandle(timestamp);
           //}
                _module.PrintQueue();
        }

        public void SendAcceptResponse(string ipAndPort)
        {
            //lock (Shared.SharedLock)
            //{
                //Send event in clock
                _module.Clock.SendEventHandle();
                var myIp = _module.LocalNodeInfo.GetIpAndPort();
                Debug.WriteLine("SERVER: " + myIp + "SEND OK TO: " + ipAndPort);
                _module.Proxy.Url = NetworkHelper.FormXmlRpcUrl(ipAndPort);
                //send accept response with parameter which describes our host
                var newSendThread = new Thread(() => _module.Proxy.GetAcceptResponse(_module.LocalNodeInfo.GetIpAndPort(), _module.Clock.Value));
                newSendThread.Start();
                //Thread.Sleep(5);
            //}
        }

        public void GetAcceptResponse(string fromIpAndPort, int timestamp)
        {
            //lock (Shared.SharedLock)
            //{
                var myIp = _module.LocalNodeInfo.GetIpAndPort();
                
                _module.RemoveFromAcceptList(fromIpAndPort);

                //check if all accept messages received. if yes, start accessing to resource
                if (_module.IsGotAllOk())
                {
                    Debug.WriteLine("RESET. GOT ALL AT: " + myIp);
                    //ManualResetEvent _isAcceptMessagesFinished.Set() in Node
                    _module.Client.HasGotAllMessagesBack.Set();
                }

                //Clock: recv handle
                _module.Clock.ReceiveEventHandle(timestamp);
            //}
        }
    }
}
