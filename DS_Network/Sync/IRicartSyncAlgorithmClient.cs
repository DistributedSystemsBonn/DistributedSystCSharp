using System.Collections.Generic;
using DS_Network.Network;

namespace DS_Network.Sync
{
    public interface IRicartSyncAlgorithmClient
    {
        /// <summary>
        /// Send request
        /// </summary>
        void SendSyncRequestToAllHosts_RA(List<NodeInfo> toSendHosts);

        /// <summary>
        /// Access critical point
        /// </summary>
        /// <returns>resource</returns>
        void Release_RA();

        void RicartReset();
    }
}