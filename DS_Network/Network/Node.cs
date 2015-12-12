using System;
using System.Collections.Generic;
using DS_Network.Helpers;

namespace DS_Network.Network
{
    /// <summary>
    /// Client host
    /// </summary>
    public class Node
    {
        private Dictionary<String, NodeInfo> _hostLookup = new Dictionary<String, NodeInfo>();
        private IConnectionProxy _client;
        private NodeInfo _nodeInfo;

        public Dictionary<String, NodeInfo> HostLookup
        {
            get
            {
                return _hostLookup;
            }
        }

        public NodeInfo NodeInfo
        {
            get
            {
                return _nodeInfo;;
            }
        }

        //TODO: put WCF service to constructor as parameter and use it in methods (like join...)
        public Node(IConnectionProxy client, int port) //ServiceReference1.Service1Client client
        {
            
            _client = client;
            var ipAddress = NetworkHelper.FindIp().ToString();
            
            _nodeInfo = new NodeInfo(ipAddress, port);
        }

        public void ProcessCommand(string command)
        {
            var commandArr = StringHelper.GetCommandArray(command);
            var commandName = commandArr[0];

            if (StringHelper.IsWithParameter(commandArr))
            {
                if (commandName != "join")
                {
                    throw new ArgumentException("Only join command can be with parameter");
                }
                var commandParameter = commandArr[1];

                String[] obj = commandParameter.Split(':');
                string ip = obj[0];
                int port = Convert.ToInt32(obj[1]);

                //IPAddress toJoinAddress = StringHelper.ConvertIpAddress(commandParameter);
                Join(commandParameter);
            }
            else if (commandArr.Length == 1)
            {
                if (commandName == "signoff")
                {
                    SignOff();
                }
                else if (commandName == "start")
                {
                    Start();
                }
                else if (commandName == "gethosts")
                {
                    PrintHosts();
                }
            }
            else
            {
                throw new ArgumentException("Number of parameters shouldn't be more than 1 or command should be without parameter");
            }
        }

        public void PrintHosts()
        {
            if (_hostLookup.Count == 0)
            {
                Console.WriteLine("List of host is empty, host is not in network");
                return;
            }

            foreach (var host in _hostLookup.Values)
            {
                Console.WriteLine(host.GetFullUrl());
            }
        }

        public void Join(String ipAndPort)
        {
            Console.WriteLine("Join operation with address " + ipAndPort);
            var toJoinInfo = new NodeInfo(ipAndPort);
            if (toJoinInfo.IsSameHost(_nodeInfo))
            {
                throw new ArgumentException("Cannot join yourself");
            }

            _client.Url = toJoinInfo.GetFullUrl();

            //Receive list from receiver
            var listsOfHosts = _client.getHosts(_nodeInfo.GetIpAndPort());

            //Add list to dictionary
            foreach (var host in listsOfHosts)
            {
                var toSendHost = AddNewHost(host.ToString());
                _client.Url = toSendHost.GetFullUrl();
                _client.addNewHost(_nodeInfo.GetIpAndPort());
            }

            //Add ipAndPort of receiver
            AddNewHost(ipAndPort);

            Console.WriteLine(listsOfHosts.ToString());
        }

        /// <summary>
        /// Add new host to dictionary
        /// </summary>
        /// <param name="ipAndPort"></param>
        /// <returns></returns>
        public NodeInfo AddNewHost(string ipAndPort)
        {
            var nodeInfo = new NodeInfo(ipAndPort);
            _hostLookup.Add(ipAndPort, new NodeInfo(ipAndPort));

            return nodeInfo;
        }

        /// <summary>
        /// Sign off from network
        /// Sends to all hosts message to remove from their dict
        /// </summary>
        public void SignOff()
        {
            var myIp = _nodeInfo.GetIpAndPort();

            foreach (var host in _hostLookup.Values)
            {
                _client.Url = host.GetFullUrl();
                //call rpc method to sign off
                _client.signOff(myIp);
            }

            _hostLookup.Clear();
        }

        public void Start()
        {
            //TODO: 1. send message to all other nodes
            //IN LOOP for 20 seconds
            //2. wait random amount of time
            //3. read string variable from master node
            //4. append some random english word to this string
            //5. write updated string to master node
            //END LOOP

            //6. Node fetches from Master node the final string
            //7. And writes this final string on screen

            //NOTE: read and write operations should be syncronized
        }

    }
}
