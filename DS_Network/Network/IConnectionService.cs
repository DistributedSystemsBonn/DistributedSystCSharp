using System;
using System.ServiceModel;
using CookComputing.XmlRpc;

namespace DS_Network.Network
{
    // NOTE: You can use the "Rename" command on the "Refactor"
    //menu to change the interface name "IService1" in both code and config file together.
    public interface IConnectionService
    {
        [XmlRpcMethod("Host.join")]
        bool join(string ipAndPort);

        //void AddNewComputer(string ip);

        [XmlRpcMethod("Host.signoff")]
        bool signOff();

        [XmlRpcMethod("Host.start")]
        bool start();

        [XmlRpcMethod("Host.gethosts")]
        //Object[] getHosts();
        bool getHosts();
    }
}

