using System;
using CookComputing.XmlRpc;

namespace DS_Network.Network
{
    public interface IConnectionService
    {
        [XmlRpcMethod("Host.join")]
        bool join(string ipAndPort);

        //void AddNewComputer(string ip);

        [XmlRpcMethod("Host.signOff")]
        bool signOff();

        [XmlRpcMethod("Host.start")]
        bool start();

        [XmlRpcMethod("Host.getHosts")]
        Object[] getHosts(String ipAndPortCallee);
    }
}

