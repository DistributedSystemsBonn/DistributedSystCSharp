using DS_Network.Helpers;

namespace DS_Network.Clock
{
    public class ExtendedLamportClock
    {
        private long _localId;
        public int Value { get; set; }

        public ExtendedLamportClock(long localId)
        {
            _localId = localId;
        }

        private void Increment()
        {
            ++Value;
        }

        public int SendEventHandle()
        {
            Increment();
            return Value;
        }

        public int LocalEventHandle()
        {
            Increment();
            return Value;
        }

        public int ReceiveEventHandle(int candidateValue)
        {
            if (candidateValue > Value)
            {
                Value = candidateValue;
            }
            Increment();
            LogHelper.WriteStatus("UPDATE CLOCK TO:" + Value);
            return Value;
        }

        public bool CompareTime(int requestLamportClock, long remoteId)
        {
            if (requestLamportClock < Value)
                return true;
            if (requestLamportClock > Value)
                return false;
            return remoteId < _localId;
        }

    }
}
