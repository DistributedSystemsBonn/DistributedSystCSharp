using System.Collections.Generic;
using DS_Network.Network;

namespace DS_Network.Election
{
    public interface IElectionAlgorithm
    {
        /// <summary>
        /// reset static variables
        /// </summary>
        void BullyReset();

        void startBullyElection(NodeInfo node, Dictionary<string, NodeInfo> hostLookup, IConnectionProxy proxy);
        void sendElectionMsg(NodeInfo node, NodeInfo target, IConnectionProxy proxy);
        void sendElectionFinalMsg(NodeInfo node, NodeInfo target, IConnectionProxy proxy);
        void finishElection();
    }
}