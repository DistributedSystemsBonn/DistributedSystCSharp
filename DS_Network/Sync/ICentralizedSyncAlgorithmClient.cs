using System.Collections.Generic;
using DS_Network.Network;

namespace DS_Network.Sync
{
    public interface ICentralizedSyncAlgorithmClient
    {
        /// <summary>
        /// Send Sync Req to Master node
        /// </summary>
        void SendSyncRequestToMaster_CT(NodeInfo masterNode);

        /// <summary>
        /// Release master resource
        /// </summary>
        void Release_CT();

        void CentralizedReset();
    }
}
