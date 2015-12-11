using System;

namespace DS_Network.Network
{
    public class NodeInfo
    {
        private string _id = String.Empty;
        private string _ip;
        private int _port;

        public int Port
        {
            get { return _port; }
        }

        public Guid? Id
        {
            get
            {
                if (_id == String.Empty)
                {
                    return null;
                }

                return new Guid(_id);
            }
            set { _ip = value.ToString(); }
        }

        public string GetIpAndPort()
        {
            return _ip + ":" + _port;
        }

        public int GetGuidCode()
        {
            Guid id = Guid.Empty;
            if (Id != null)
            {
                id = (Guid) Id;
            }
            else
            {
                throw new ArgumentException("Guid cannot be null when getting code");
            }

            var chars = id.ToByteArray();
            return BitConverter.ToInt32(chars, 0);
        }

        public int Compare(NodeInfo host2)
        {
            int myGuidCode = GetGuidCode();
            int host2GuidCode = host2.GetGuidCode();
            if (myGuidCode < host2GuidCode)
                return -1;
            if (myGuidCode == host2GuidCode)
                return 0;
            return 1;
        }

        public bool IsSameHost(NodeInfo host)
        {
            return Id == host.Id;
        }

        public new string ToString()
        {
            return "http://" + _ip + ":" + _port + "/";
        }

        public string GetFullUrl()
        {
            return ToString() + "xmlrpc";
        }

        public NodeInfo(string id, string ip, int port)
        {
            _id = id;
            _ip = ip;
            _port = port;
        }

        public NodeInfo(string ipAndPort)
        {
            String[] obj = ipAndPort.Split(':');
            _ip = obj[0];
            _port = Convert.ToInt32(obj[1]);
        }
    }
}
