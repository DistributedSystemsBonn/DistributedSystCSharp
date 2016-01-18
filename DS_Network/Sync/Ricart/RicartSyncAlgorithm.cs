using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using DS_Network.Clock;
using DS_Network.Helpers;
using DS_Network.Network;

namespace DS_Network.Sync.Ricart
{
    public class RicartSyncAlgorithm
    {
        private ExtendedLamportClock _clock;
        public ExtendedLamportClock Clock
        {
            get
            {
                return _clock;
            }
        }

        private bool _isInterested;
        public bool IsInterested
        {
            get { return _isInterested; }
            set
            {
                //Is interested - local event
                Clock.LocalEventHandle();
                _isInterested = value;
            }
        }

        public NodeInfo LocalNodeInfo { get; private set; }
        public IConnectionProxy Proxy { get; private set; }
        public long LocalId { get; private set; }

        /// <summary>
        /// Need this list to ensure that client got all accept messages
        /// Contains: ip and port to which host we sent message
        /// </summary>
        private List<string> _acceptList = new List<string>();

        #region AcceptList
        /// <summary>
        /// to check if it got all response for sent requests.
        /// </summary>
        public bool IsGotAllOk()
        {
            return _acceptList.Count == 0;
        }

        /// <summary>
        /// Add item into list
        /// </summary>
        public void AddToAcceptList(string ipAndPort)
        {
           _acceptList.Add(ipAndPort);
        }
        /// <summary>
        /// remove item from list
        /// </summary>
        public void RemoveFromAcceptList(string ipAndPort)
        {
            Console.WriteLine("SERVER: " + LocalId + " REMOVE IP: " + ipAndPort);
            if (!_acceptList.Remove(ipAndPort))
            {
                throw new ArgumentException("Element in accept list doesnt exist: " + ipAndPort);
            }
        }
        #endregion

        public AccessState State { get; set; }

        private List<DataRequest> _queue = new List<DataRequest>();
        public List<DataRequest> Queue
        {
            get
            {
                return _queue;
            }
        }

        #region Request _queue

        /// <summary>
        /// Add a request into queue and order it.
        /// </summary>
        public void AddRequest(DataRequest request)
        {
            LogHelper.WriteStatus("Server: Add request to queue: " + request.CallerId + " with timestamp: " + request.Time);
            _queue.Add(request);
            _queue = _queue.OrderBy(x => x.Time).ToList();
            
            PrintQueue();
        }
        /// <summary>
        /// pop a request from queue
        /// </summary>
        public void PopRequest(DataRequest request)
        {
            _queue.Remove(request);
            LogHelper.WriteStatus("Server: Remove request from queue: " + request.CallerId + " with timestamp: " + request.Time);
            PrintQueue();
        }

        public int GetQueueCount()
        {
            return _queue.Count;
        }

        public void PrintQueue()
        {
            string print = "-- Queue::\n";
            //Console.Write("-- Queue:: ");
            int i = 1;
            foreach (var q in Queue)
            {
                print += i++.ToString() + ": " + q.IpAndPort + ", " + q.Time + "\n";
            }
            Console.WriteLine(print + "\n");
            //Console.WriteLine("");
        }

        #endregion Request _queue

        public RicartSyncAlgorithmClient Client { get; private set; }
        public RicartSyncAlgorithmServer Server { get; private set; }

        /// <summary>
        /// constructor
        /// </summary>
        public RicartSyncAlgorithm(NodeInfo nodeInfo, IConnectionProxy proxy)
        {
            State = AccessState.Released;
            LocalId = nodeInfo.Id;
            Proxy = proxy;
            LocalNodeInfo = nodeInfo;
            _clock = new ExtendedLamportClock(LocalId);

            Client = new RicartSyncAlgorithmClient(this);
            Server = new RicartSyncAlgorithmServer(this);
        }

        /// <summary>
        /// Reset variables to start again
        /// </summary>
        public void RicartReset()
        {
            _clock = new ExtendedLamportClock(LocalId);
            State = AccessState.Released;
            _isInterested = false;
            _acceptList = new List<string>();
            _queue = new List<DataRequest>();
        }
    }
}
