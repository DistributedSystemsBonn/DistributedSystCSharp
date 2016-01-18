using System.Collections.Generic;
using System.Threading;
using DS_Network.Network;

namespace DS_Network.Sync.Centralized
{
    public class CentralizedSyncAlgorithmClient : ICentralizedSyncAlgorithmClient
    {

        public ManualResetEvent IsAllowed = new ManualResetEvent(false);
        private CentralizedSyncAlgorithm _module;

        public CentralizedSyncAlgorithmClient(CentralizedSyncAlgorithm module)
        {
            _module = module;
        }

        /// <summary>
        /// Send Sync Req to Master node
        /// </summary>
        public void SendSyncRequestToMaster_CT(NodeInfo masterNode)
        {
            IsAllowed.Reset();
            _module.Proxy.Url = masterNode.GetFullUrl();
            // to evade time out - using thread and wait for an acceptance trigger.
            var send = new Thread(
                        () => SendSyncMsg(_module.Proxy, masterNode));
            send.Start();

            // wait until the master allow to accept to the resource
            IsAllowed.WaitOne();
        }

        /// <summary>
        /// Send Sync Req to Master node
        /// </summary>
        private void SendSyncMsg(IConnectionProxy proxy, NodeInfo toNode) 
        {
            proxy.Url = toNode.GetFullUrl();
            _module.Proxy.GetSyncRequest_CT(_module.LocalNodeInfo.Id, _module.LocalNodeInfo.GetIpAndPort());
        }
        
        /// <summary>
        /// Release master resource
        /// </summary>
        public void Release_CT() 
        {
            _module.Proxy.GetReleasedMsg_CT(_module.LocalNodeInfo.Id, _module.LocalNodeInfo.GetIpAndPort());
        }

        public void CentralizedReset()
        {
            _module.CentralizedReset();
        }
    }
}
