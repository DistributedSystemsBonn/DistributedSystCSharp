using System;
using System.Net;

namespace DS_Network.Network
{
    public class NodeInfo
    {
        private string _id;
        private string _ip;

        public Guid Ip
        {
            get
            {
                return new Guid(_ip);
            }
            set { _ip = value.ToString(); }
        }

        public NodeInfo(string id, string ip)
        {
            _id = id;
            ip = ip;
        }
    }
}
