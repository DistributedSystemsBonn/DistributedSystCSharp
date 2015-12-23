using System;
using System.Collections.Generic;
using System.Linq;
using DS_Network.Helpers;
using DS_Network.Network;

namespace DS_Network.Sync.Ricart
{
    public class RicartSyncAlgorithm
    {
        private bool _isInterested;

        public bool IsInterested
        {
            get { return _isInterested; }
            set
            {
                //Is interested - local event
                IncrementLamportClock();
                _isInterested = value;
            }
        }

        public int LamportClock { get; set; }

        /// <summary>
        /// Need this list to ensure that client got all accept messages
        /// Contains: ip and port to which host we sent message
        /// </summary>
        public List<string> AcceptList { get; set; } 

        public AccessState State { get; set; }

        public Object QueueLock = new object();

        public List<DataRequest> Queue { get; set; }

        #region Lamport Clock

        public int IncrementLamportClock()
        {
            return ++LamportClock;
        }

        public int UpdateClock(int candidateValue)
        {
            if (candidateValue > LamportClock)
            {
                LamportClock = candidateValue;
            }
            IncrementLamportClock();
            LogHelper.WriteStatus("Update clock to " + LamportClock);
            return LamportClock;
        }

        #endregion Lamport Clock

        #region Request Queue

        public void AddRequest(DataRequest request)
        {
            LogHelper.WriteStatus("Server: Add request to queue: " + request.CallerId + " with timestamp: " +
                                          request.Time);
            Queue.Add(request);
            Queue = Queue.OrderBy(x => x.Time).ToList();
            //Queue = Queue.OrderBy(x => x.Time).ThenBy(x => x.CallerId).ToList();
        }

        public void PopRequest(DataRequest request)
        {
            Queue.Remove(request);
                LogHelper.WriteStatus("Server: Remove request from queue: " + request.CallerId + " with timestamp: " +
                                          request.Time);
        }

        public int GetQueueCount()
        {
            return Queue.Count;
        }

        #endregion Request Queue

        public RicartSyncAlgorithmClient Client { get; private set; }
        public RicartSyncAlgorithmServer Server { get; private set; }

        public RicartSyncAlgorithm(NodeInfo nodeInfo, IConnectionProxy proxy)
        {
            State = AccessState.Released;
            Queue = new List<DataRequest>();
            Client = new RicartSyncAlgorithmClient(this);
            Server = new RicartSyncAlgorithmServer(this);
            AcceptList = new List<string>();
            LocalId = nodeInfo.Id;
            Proxy = proxy;
            LocalNodeInfo = nodeInfo;
        }

        public NodeInfo LocalNodeInfo { get; private set; }

        public IConnectionProxy Proxy { get; private set; }

        public long LocalId { get; private set; }
    }
}
