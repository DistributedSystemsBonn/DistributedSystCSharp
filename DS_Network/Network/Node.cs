using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using DS_Network.Helpers;
using System.Threading;
using DS_Network.Election;
using DS_Network.Sync;

namespace DS_Network.Network
{
    /// <summary>
    /// Client host
    /// </summary>
    public class Node
    {
        private Dictionary<String, NodeInfo> _hostLookup = new Dictionary<String, NodeInfo>();
        private HashSet<string> _appendedStringSet = new HashSet<string>(); 
        private IConnectionProxy _proxy;
        private NodeInfo _nodeInfo;
        private static DateTime _startTime;
        private static IElectionAlgorithm _electionAlgorithm;
        private NodeInfo _masterNode;
        private static TimeSpan _maxDuration = new TimeSpan(0, 0, 20);
        private static AccessState _state = AccessState.Released;
        private ISyncAlgorithmClient _syncAlgorithm;

        public string Resource { get; set; }

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
                return _nodeInfo;
            }
        }

        public Node(NodeInfo nodeInfo, IConnectionProxy client, IElectionAlgorithm electionAlgorithm, ISyncAlgorithmClient syncAlgorithmClient, int port) //ServiceReference1.Service1Client client
        {
            _proxy = client;
            _nodeInfo = nodeInfo;
            _electionAlgorithm = electionAlgorithm;
            _syncAlgorithm = syncAlgorithmClient;
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
                else if (commandName == "election")
                {
                    ElectMasterNode();
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

            _proxy.Url = toJoinInfo.GetFullUrl();

            //Receive list from receiver
            var listsOfHosts = _proxy.getHosts(_nodeInfo.GetIpAndPort());

            //Add list to dictionary
            foreach (var host in listsOfHosts)
            {
                var toSendHost = AddNewHost(host.ToString());
                _proxy.Url = toSendHost.GetFullUrl();
                _proxy.addNewHost(_nodeInfo.GetIpAndPort());
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
                _proxy.Url = host.GetFullUrl();
                //call rpc method to sign off
                _proxy.signOff(myIp);
            }

            _hostLookup.Clear();
        }

        /// <summary>
        /// 
        /// </summary>
        public void Start()
        {
            if (_hostLookup.Count == 0)
            {
                Console.WriteLine("This client is not in network");
                return;
            }
            //if (_masterNode == null)
            //{
            //    Console.WriteLine("No master node is elected in the network");
            //    Console.WriteLine("Please execute election command to elect master node in the network");
            //    return;
            //}

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

            ElectMasterNode();
        }

        /// <summary>
        /// Start Election of Master Node
        /// </summary>
        public void ElectMasterNode()
        {
            if (_hostLookup.Count == 0)
            {
                Console.WriteLine("This client is not in network");
                return;
            }

            // TODO : master node reset
            _masterNode = null;

            _electionAlgorithm.startBullyElection(_nodeInfo, _hostLookup, _proxy);
        }

        public void ElectMasterNodeByReceivingMsg(string id)
        {
            Console.WriteLine("Received Election message from " + id);

            _electionAlgorithm.startBullyElection(_nodeInfo, _hostLookup, _proxy);
        }

        public void SetMasterNode(String ipAndPortMaster)
        {
            _masterNode = new NodeInfo(ipAndPortMaster);

            Console.WriteLine("Master is elected: " + _masterNode.GetIpAndPort());

            _electionAlgorithm.finishElection();
            //Thread.Sleep(10000);
            //START ALGORITHM. Because we know our master node. 
            //StartAlgorithm();
            Thread startAlgorithm = new Thread(() => StartAlgorithm());
            startAlgorithm.Start();

        }

        private string GetRandomFruit()
        {
            Random rnd = new Random();

            string[] fruits = { "apple", "mango", "papaya", "banana", "guava", "pineapple" };
            return fruits[rnd.Next(0, fruits.Length)];
        }

        public void StartAlgorithm()
        {
            var rnd = new Random();
            int count = 0;

            if (IsMasterNode())
            {
                return;
            }

            do
            {
                count++;
                Console.WriteLine("Start iteration #" + count);

                //wait random amount of time
                var sleepTime = rnd.Next(2000, 5001); //generate a random waiting time between 2s and 4s
                Console.WriteLine("Start waiting for " + sleepTime / 1000 + " sec");
                Thread.Sleep(sleepTime);
                Console.WriteLine("Finished waiting for " + sleepTime / 1000 + " sec");
                //initialize client to communicate with master node
                _proxy.Url = _masterNode.GetFullUrl();

                //read resource
                LogHelper.WriteStatus("Reading resource from mn " + _masterNode.GetIpAndPort());
                _syncAlgorithm.SendSyncRequest(_proxy, _hostLookup.Values.ToList());

                var readResFromMn = _proxy.readResource(_nodeInfo.GetIpAndPort());

                _syncAlgorithm.Release();

                //generate string
                var randomStr = GetRandomFruit();
                LogHelper.WriteStatus("Generated string: " + randomStr);

                //append string
                var appendedString = String.Concat(readResFromMn, randomStr);
                //add appended string to list
                _appendedStringSet.Add(appendedString);
                LogHelper.WriteStatus("Result of appended string " + appendedString);

                //write updated string to the master node
                _syncAlgorithm.SendSyncRequest(_proxy, _hostLookup.Values.ToList());

                _proxy.updateResource(appendedString, _nodeInfo.GetIpAndPort());
                _syncAlgorithm.Release();
                LogHelper.WriteStatus("Updated string on mn " + _masterNode.GetIpAndPort());


                var executeTime = DateTime.Now;
                if (TimeSpan.Compare(executeTime - _startTime, _maxDuration) >= 0)
                {
                    Console.WriteLine("Exited loop with " + (executeTime - _startTime));
                    break;
                }
            } while (true);

            var finalString = _proxy.readResource(_nodeInfo.GetIpAndPort());
            Console.WriteLine("Final string: " + finalString);
        }

        public bool IsMasterNode()
        {
            return _nodeInfo.IsSameHost(_masterNode);
        }


    }
}
