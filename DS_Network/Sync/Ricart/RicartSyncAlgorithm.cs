using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using DS_Network.Clock;
using DS_Network.Helpers;
using DS_Network.Network;

namespace DS_Network.Sync.Ricart
{
    public static class Shared
    {
        public static readonly object SharedLock = new object();
        public static readonly object RecvLock = new object();
        public static readonly object SendLock = new object();
    }

    public class RicartSyncAlgorithm
    {
        private bool _isInterested;

        private ExtendedLamportClock _clock;

        public ExtendedLamportClock Clock
        {
            get
            {
                return _clock;
            }
        }

        public bool IsInterested
        {
            get { return _isInterested; }
            set
            {
                //Is interested - local event
                //IncrementLamportClock();
                Clock.LocalEventHandle();
                _isInterested = value;
            }
        }

        /// <summary>
        /// Need this list to ensure that client got all accept messages
        /// Contains: ip and port to which host we sent message
        /// </summary>
        private List<string> _acceptList = new List<string>();

        public bool IsGotAllOk()
        {
            return _acceptList.Count == 0;
        }

        public void AddToAcceptList(string ipAndPort)
        {
            //lock (Shared.SharedLock)
            //{
                _acceptList.Add(ipAndPort);
            //}
        }

        public void RemoveFromAcceptList(string ipAndPort)
        {
            //lock (Shared.SharedLock)
            //{
                Debug.WriteLine("SERVER: " + LocalId + "REMOVE IP: " + ipAndPort);

                if (!_acceptList.Remove(ipAndPort))
                {
                    throw new ArgumentException("Element in accept list doesnt exist: " + ipAndPort);
                }
            //}
        }

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

        public void AddRequest(DataRequest request)
        {
            //lock (Shared.SharedLock)
            //{
                LogHelper.WriteStatus("Server: Add request to queue: " + request.CallerId + " with timestamp: " +
                                          request.Time);
                _queue.Add(request);
                _queue = _queue.OrderBy(x => x.Time).ToList();
                //_queue = _queue.OrderBy(x => x.Time).ThenBy(x => x.CallerId).ToList();
            //}
        }

        public void PopRequest(DataRequest request)
        {
            //lock (Shared.SharedLock)
            //{
                _queue.Remove(request);
                LogHelper.WriteStatus("Server: Remove request from queue: " + request.CallerId + " with timestamp: " +
                                          request.Time);
            //}
        }

        public int GetQueueCount()
        {
            return _queue.Count;
        }

        public void PrintQueue()
        {
            Console.WriteLine("Queue:: ");
            foreach (var q in _queue) 
            {
                Console.Write(q.ipAndPort);
                Console.Write(", ");
            }
            Console.WriteLine("");
        }

        #endregion Request _queue

        public RicartSyncAlgorithmClient Client { get; private set; }
        public RicartSyncAlgorithmServer Server { get; private set; }

        public RicartSyncAlgorithm(NodeInfo nodeInfo, IConnectionProxy proxy)
        {
            State = AccessState.Released;
            Client = new RicartSyncAlgorithmClient(this);
            Server = new RicartSyncAlgorithmServer(this);
            //_acceptList = new List<string>();
            LocalId = nodeInfo.Id;
            Proxy = proxy;
            LocalNodeInfo = nodeInfo;
            _clock = new ExtendedLamportClock(LocalId);
        }

        public NodeInfo LocalNodeInfo { get; private set; }

        public IConnectionProxy Proxy { get; private set; }

        public long LocalId { get; private set; }
    }
}
