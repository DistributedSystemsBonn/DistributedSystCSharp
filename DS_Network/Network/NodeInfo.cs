using System;

namespace DS_Network.Network
{
    public class NodeInfo
    {
        private long _id = 0;
        private string _ip;
        private int _port;

        public int Port
        {
            get { return _port; }
        }

        public long Id
        {
            get
            {
                return _id;
            }
        }

        public string GetIpAndPort()
        {
            return _ip + ":" + _port;
        }

        public int Compare(NodeInfo host2)
        {
            if (_id < host2.Id)
                return -1;
            if (_id == host2.Id)
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

        public void InitId(string ip, int port)
        {
            var parts = ip.Split('.', ':');
            var id = String.Empty;
            for (int i = 0; i < parts.Length; i++)
                id += parts[i];
            id += port.ToString();
            _id = Convert.ToInt64(id);
        }

        public NodeInfo(string ip, int port)
        {
            _ip = ip;
            _port = port;
            InitId(ip, port);
        }

        public NodeInfo(string ipAndPort)
        {
            var obj = ipAndPort.Split(':');
            _ip = obj[0];
            _port = Convert.ToInt32(obj[1]);
            InitId(_ip, _port);
        }
    }
}
