using System.Collections.Generic;
using DS_Network.Network;

namespace DS_Network.Election
{
    public interface IElectionAlgorithm
    {
        void BullyReset();
        void StartBullyElection(Dictionary<string, NodeInfo> hostLookup);
        void SendElectionMsg(NodeInfo target, IConnectionProxy proxy);
        void SendElectionFinalMsg(NodeInfo target);
        void FinishElection();
    }
}