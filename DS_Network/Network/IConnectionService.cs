using System.ServiceModel;

namespace DS_Network.Network
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface IConnectionService
    {
        [OperationContract]
        void handleJoin(string ip);

        [OperationContract]
        void AddNewComputer(string ip);

        void SignOff();

        // TODO: Add your service operations here
    }
}
