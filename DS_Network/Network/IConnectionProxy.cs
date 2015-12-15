using System;
using CookComputing.XmlRpc;

namespace DS_Network.Network
{
    public interface IConnectionProxy : IXmlRpcProxy, IConnectionService
    {
    }
}
