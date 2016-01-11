using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DS_Network.Helpers;
using DS_Network.Network;

namespace DS_Network.Sync.Centralized
{
    public class CentralizedSyncAlgorithm
    {
        public AccessState State { get; set; }
        public List<DataRequest> Queue { get; set; }
        public NodeInfo LocalNodeInfo { get; private set; }
        public IConnectionProxy Proxy { get; private set; }
        public long LocalId { get; private set; }

        public CentralizedSyncAlgorithmClient Client { get; private set; }
        public CentralizedSyncAlgorithmServer Server { get; private set; }

        
        public CentralizedSyncAlgorithm(NodeInfo nodeInfo, IConnectionProxy proxy)
        {
            // State and Queue is only for Master Node
            State = AccessState.Released;
            Queue = new List<DataRequest>();
            
            Client = new CentralizedSyncAlgorithmClient(this);
            Server = new CentralizedSyncAlgorithmServer(this);

            LocalId = nodeInfo.Id;
            Proxy = proxy;
            LocalNodeInfo = nodeInfo;
        }

        public void AddRequest(DataRequest request)
        {
            Queue.Add(request);
            //Queue = Queue.OrderBy(x => x.Time).ToList();
            //Queue = Queue.OrderBy(x => x.Time).ThenBy(x => x.CallerId).ToList();
        }

        public DataRequest PopRequest()
        {
            DataRequest firstNode = null;
            if (Queue.Count > 0)
            {
                firstNode = Queue[0];
                Queue.Remove(firstNode);
                //LogHelper.WriteStatus("Server: Remove request from queue: " + request.CallerId + " with timestamp: " +
                //                          request.Time);
            }
            return firstNode;
        }
    }
}
