using System;
using CookComputing.XmlRpc;
using DS_Network.Sync;

namespace DS_Network.Network
{
    public interface IConnectionProxy : IXmlRpcProxy, IConnectionService, ISyncAlgorithmServer
    {
    }
}
