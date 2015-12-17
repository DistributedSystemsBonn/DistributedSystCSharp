using CookComputing.XmlRpc;

namespace DS_Network.Sync
{
    public interface ISyncAlgorithmServer
    {
        /// <summary>
        /// Call handler of server
        /// HANDLE
        /// </summary>
        [XmlRpcMethod("Host.getSyncRequest")]
        bool GetSyncRequest(int timestamp, long id);
    }
}
