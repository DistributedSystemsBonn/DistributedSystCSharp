using CookComputing.XmlRpc;

namespace DS_Network.Sync
{
    public interface IRicartSyncAlgorithmServer
    {
        /// <summary>
        /// Call handler of server
        /// HANDLE
        /// </summary>
        [XmlRpcMethod("Host.getSyncRequestRA")]
        void GetSyncRequest_RA(string timestamp, string id, string ipAndPort);

        [XmlRpcMethod("Host.getAcceptResponseRA")]
        void GetAcceptResponse_RA(string fromIpAndPort, int timestamp);
    }
}