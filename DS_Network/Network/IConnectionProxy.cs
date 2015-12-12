using CookComputing.XmlRpc;

namespace DS_Network.Network
{
    public interface IConnectionProxy : IXmlRpcProxy
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
