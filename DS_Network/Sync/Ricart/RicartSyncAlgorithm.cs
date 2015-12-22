using System;
using System.Collections.Generic;
using System.Linq;
using DS_Network.Helpers;

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
            if (Queue.Exists(x => x.Id == request.Id))
            {
                return;
            }

            LogHelper.WriteStatus("Server: Add request to queue: " + request.CallerId + " with timestamp: " +
                                          request.Time);
            Queue.Add(request);
            Queue = Queue.OrderBy(x => x.Time).ThenBy(x => x.CallerId).ToList();
        }

        public void PopRequest(DataRequest request)
        {
            if (Queue.Contains(request))
            {
                Queue.Remove(request);
                LogHelper.WriteStatus("Server: Remove request from queue: " + request.CallerId + " with timestamp: " +
                                          request.Time);
            }
        }

        public int GetQueueCount()
        {
            return Queue.Count;
        }

        #endregion Request Queue

        public RicartSyncAlgorithmClient Client { get; set; }
        public RicartSyncAlgorithmServer Server { get; set; }

        public RicartSyncAlgorithm(long localId)
        {
            State = AccessState.Released;
            Queue = new List<DataRequest>();
            Client = new RicartSyncAlgorithmClient(this);
            Server = new RicartSyncAlgorithmServer(this, localId);
        }
    }
}
