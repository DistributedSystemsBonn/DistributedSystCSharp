using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using DS_Network.Network;

namespace DS_Network.Sync.Centralized
{
    public class CentralizedSyncAlgorithmClient : ISyncAlgorithmClient
    {

        public ManualResetEvent _isAllowed = new ManualResetEvent(false);
        
        private CentralizedSyncAlgorithm _module;
        public CentralizedSyncAlgorithmClient(CentralizedSyncAlgorithm module)
        {
            _module = module;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="masterNode"></param>
        public void SendSyncRequestToMaster(NodeInfo masterNode)
        {
            _isAllowed.Reset();
            _module.Proxy.Url = masterNode.GetFullUrl();
            var newThread = new Thread(
                        () => SendSyncMsg(_module.Proxy, masterNode));
            newThread.Start();

            // wait until the master allow to accept to the resource
            _isAllowed.WaitOne();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="proxy"></param>
        /// <param name="toNode"></param>
        private void SendSyncMsg(IConnectionProxy proxy, NodeInfo toNode) 
        {
            proxy.Url = toNode.GetFullUrl();
            _module.Proxy.GetSyncRequest(0, toNode.Id, _module.LocalNodeInfo.GetIpAndPort());
        }


        public void SendSyncRequestToAllHosts(List<NodeInfo> toSendHosts) { }
        
        /// <summary>
        /// 
        /// </summary>
        public void Release() 
        {
            _module.Proxy.GetReleasedMsg(_module.LocalNodeInfo.GetIpAndPort());
        }
    }
}
