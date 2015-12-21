using System.Threading;
using DS_Network.Helpers;

namespace DS_Network.Sync.Ricart
{
    public class RicartSyncAlgorithmServer : ISyncAlgorithmServer
    {
        private RicartSyncAlgorithm _module;
        private long _localId;

        public RicartSyncAlgorithmServer(RicartSyncAlgorithm module, long localId)
        {
            _module = module;
            _localId = localId;
        }

        public bool GetSyncRequest(int timestamp, long id)
        {
            LogHelper.WriteStatus("Server: Start handling request from: " + id + " with timestamp: " + timestamp);

            _module.UpdateClock(timestamp);

            // create request object
            var request = new DataRequest()
            {
                Time = timestamp,
                Id = _localId,
                CallerId = id
            };

            while (true)
            {
                if (_module.State != AccessState.Held && !_module.IsInterested)
                {
                    _module.PopRequest(request);
                    return true;
                }

                if (_module.State == AccessState.Held)
                {
                    _module.AddRequest(request);
                }
                else if (_module.IsInterested)
                {
                    if (_module.RequestTime < timestamp)
                    {
                        _module.PopRequest(request);
                        return true;
                    }

                    _module.AddRequest(request);
                }
                Thread.Sleep(5);
            }
        }
    }
}
