using System.Collections.Generic;
using DS_Network.Helpers;
using DS_Network.Network;
using System;

namespace DS_Network.Sync.Centralized
{
    public class CentralizedSyncAlgorithm
    {
        public AccessState State { get; set; }
        private List<DataRequest> _queue = new List<DataRequest>();
        public List<DataRequest> Queue
        {
            get
            {
                return _queue;
            }
        }
        public NodeInfo LocalNodeInfo { get; private set; }
        public IConnectionProxy Proxy { get; private set; }

        public CentralizedSyncAlgorithmClient Client { get; private set; }
        public CentralizedSyncAlgorithmServer Server { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public CentralizedSyncAlgorithm(NodeInfo nodeInfo, IConnectionProxy proxy)
        {
            // State and Queue is only for Master Node
            State = AccessState.Released;
            Proxy = proxy;
            LocalNodeInfo = nodeInfo;

            Client = new CentralizedSyncAlgorithmClient(this);
            Server = new CentralizedSyncAlgorithmServer(this);
        }

        /// <summary>
        /// Add request into queue
        /// </summary>
        public void AddRequest(DataRequest request)
        {
            _queue.Add(request);
            LogHelper.WriteStatus("Master: Added " + request.CallerId + " into the queue.");
            PrintQueue();
        }
        /// <summary>
        /// pop request
        /// </summary>
        public DataRequest PopRequest()
        {
            DataRequest firstNode = null;
            if (_queue.Count > 0)
            {
                firstNode = _queue[0];
                _queue.Remove(firstNode);
                LogHelper.WriteStatus("Server: Remove request from queue: " + firstNode.CallerId);
            }
            PrintQueue();
            return firstNode;
        }

        /// <summary>
        /// list items in Queue
        /// </summary>
        public void PrintQueue()
        {
            string print = "-- Queue::";
            //Console.Write("-- Queue:: ");
            foreach (var q in _queue)
            {
                print += q.IpAndPort + ", ";
                //Console.Write(q.IpAndPort);
                //Console.Write(", ");
            }
            Console.WriteLine(print + "\n");
            //Console.WriteLine("");
        }

        public void CentralizedReset()
        {
            State = AccessState.Released;
        }
    }
}
