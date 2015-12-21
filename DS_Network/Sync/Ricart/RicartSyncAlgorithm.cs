using System;
using System.Collections.Generic;
using System.Linq;
using DS_Network.Helpers;

namespace DS_Network.Sync.Ricart
{
    public class RicartSyncAlgorithm
    {
        public AccessState State { get; set; }
        //TODO: queue
        public int RequestTime;

        public Object QueueLock = new object();

        public int ExactTime { get; private set; }

        private DateTime _lastRequestTS;

        private static List<DataRequest> Queue { get; set; }

        #region Lamport Clock

        public int IncrementClock()
        {
            return ++ExactTime;
        }

        public int UpdateClock(int candidateValue)
        {
            if (candidateValue > ExactTime)
                ExactTime = candidateValue;
            LogHelper.WriteStatus("Update clock to " + ExactTime);
            return ExactTime;
        }

        #endregion Lamport Clock

        #region Request Queue

        public void AddRequest(DataRequest request)
        {
            //lock (QueueLock)
            //{
            LogHelper.WriteStatus("Server: Add request to queue: " + request.CallerId + " with timestamp: " +
                                          request.Time);
            Queue.Add(request);
            Queue = Queue.OrderBy(x => x.Time).ThenBy(x => x.Time).ToList();

                //Queue = Queue.OrderBy(x => x.Time).ThenBy(x => x.CallerId).ToList();
            //}
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

        public long TimeFromRequest()
        {
            return (DateTime.Now.Ticks - _lastRequestTS.Ticks) / TimeSpan.TicksPerMillisecond;
        }

        #endregion Request Queue

        public RicartSyncAlgorithmClient Client { get; set; }
        public RicartSyncAlgorithmServer Server { get; set; }
        public bool IsInterested { get; set; }

        public RicartSyncAlgorithm(long localId)
        {
            State = AccessState.Released;
            Queue = new List<DataRequest>();
            Client = new RicartSyncAlgorithmClient(this);
            Server = new RicartSyncAlgorithmServer(this, localId);
        }
    }
}
