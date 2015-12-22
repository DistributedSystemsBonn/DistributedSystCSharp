using System.Collections.Generic;
using DS_Network.Network;

namespace DS_Network.Sync
{
    public interface ISyncAlgorithmClient
    {
        /// <summary>
        /// Send request
        /// </summary>
        void SendSyncRequestToAllHosts(IConnectionProxy proxy, List<NodeInfo> toSendHosts);

        /// <summary>
        /// Access critical point
        /// </summary>
        /// <returns>resource</returns>
        void Release();
    }
}


