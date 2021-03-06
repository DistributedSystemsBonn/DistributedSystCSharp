﻿using CookComputing.XmlRpc;

namespace DS_Network.Sync
{
    public interface ICentralizedSyncAlgorithmServer
    {
        /// <summary>
        /// Call handler of server
        /// HANDLE
        /// </summary>
        [XmlRpcMethod("Host.getSyncRequestCT")]
        void GetSyncRequest_CT(string id, string ipAndPort);

        [XmlRpcMethod("Host.getReleasedMsgCT")]
        void GetReleasedMsg_CT(string id, string fromIpAndPort);

        [XmlRpcMethod("Host.getAcceptResponseCT")]
        void GetAcceptResponse_CT();
    }
}
