using System;
using CookComputing.XmlRpc;

namespace DS_Network.Network
{
    public interface IConnectionService
    {
        [XmlRpcMethod("Host.getHosts")]
        Object[] GetHosts(string ipAndPortCallee);

        [XmlRpcMethod("Host.addNewHost")]
        void AddNewHost(string ipAndPort);

        [XmlRpcMethod("Host.signOff")]
        bool SignOff(string ipAndPort);
        

        [XmlRpcMethod("Host.getStartMsg")]
        void GetStartMsg(bool isRicartAlgorithm);


        [XmlRpcMethod("Host.receiveElectionMsg")]
        bool ReceiveElectionMsg(string id);

        [XmlRpcMethod("Host.setMasterNode")]
        void SetMasterNode(string ipAndPort);



        [XmlRpcMethod("Host.readResource")]
        string ReadResource(string ipAndPort);

        [XmlRpcMethod("Host.updateResource")]
        void UpdateResource(string updateStr, string ipAndPort);
    }
}

