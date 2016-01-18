using System;
using System.Collections.Generic;
using System.Linq;
using DS_Network.Helpers;
using System.Threading;
using DS_Network.Election;
using DS_Network.Sync;

namespace DS_Network.Network
{
    public static class Shared
    {
        public static readonly object SharedLock = new object();
        public static readonly object RecvLock = new object();
        public static readonly object SendLock = new object();
    }

    /// <summary>
    /// Client host
    /// </summary>
    public class Node
    {
        private Dictionary<String, NodeInfo> _hostLookup = new Dictionary<String, NodeInfo>();
        
        private IConnectionProxy _proxy;
        private NodeInfo _nodeInfo;
        private NodeInfo _masterNode;
        
        private IElectionAlgorithm _electionAlgorithm;
        private IRicartSyncAlgorithmClient _ricartSyncAlgClient;
        private ICentralizedSyncAlgorithmClient _centralizedSyncAlgClient;

        private HashSet<string> _appendedStringSet = new HashSet<string>();
        private DateTime _startTime;
        private TimeSpan _maxDuration = new TimeSpan(0, 0, 10);

        private ManualResetEvent _isElectionFinished = new ManualResetEvent(false);
        private ManualResetEvent _isProcessFinished = new ManualResetEvent(false);

        public string Resource { get; set; }    // Master Resource
        private List<NodeInfo> hostlistWithoutMaster;
        //public NodeInfo MasterNode
        //{
        //    get { return _masterNode; }
        //}

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

        /// <summary>
        /// constructor
        /// </summary>
        public Node(NodeInfo nodeInfo, IConnectionProxy proxy, IElectionAlgorithm electionAlgorithm
            , IRicartSyncAlgorithmClient ricartSyncAlgorithmClient, ICentralizedSyncAlgorithmClient centralizedSyncAlgorithmClient)
        {
            _proxy = proxy;
            _nodeInfo = nodeInfo;
            _electionAlgorithm = electionAlgorithm;
            _ricartSyncAlgClient = ricartSyncAlgorithmClient;
            _centralizedSyncAlgClient = centralizedSyncAlgorithmClient;
        }

        /// <summary>
        /// command process - execute method for the command
        /// </summary>
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
                Join(commandParameter);
            }
            else if (commandArr.Length == 1)
            {
                if (commandName == "signoff")
                {
                    SignOff();
                }
                else if (commandName == "gethosts")
                {
                    PrintHosts();
                }
                //else if (commandName == "election")
                //{
                //    ElectMasterNode();
                //}
                else if (commandName == "start_ct") // centralized M.E.
                {
                    _isProcessFinished.Reset();
                    Start(false);
                    _isProcessFinished.WaitOne();
                }
                else if (commandName == "start_ra") // ricart agrawala M.E.
                {
                    _isProcessFinished.Reset();
                    Start(true);
                    _isProcessFinished.WaitOne();
                }
            }
            else
            {
                throw new ArgumentException("Number of parameters shouldn't be more than 1 or command should be without parameter");
            }
            //Thread.Sleep(10);
        }

        #region Command: gethosts

        /// <summary>
        /// Command: gethosts
        /// </summary>
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
        
        #endregion

        #region Command: join

        /// <summary>
        /// Command: Join
        /// connect to other network
        /// </summary>
        public void Join(string ipAndPort)
        {
            Console.WriteLine("# Join operation with address " + ipAndPort);
            var toJoinInfo = new NodeInfo(ipAndPort);
            if (toJoinInfo.IsSameHost(_nodeInfo))
            {
                throw new ArgumentException("Cannot join yourself");
            }

            _proxy.Url = toJoinInfo.GetFullUrl();

            // Receive list from receiver
            var listsOfHosts = _proxy.GetHosts(_nodeInfo.GetIpAndPort());

            // Add list to dictionary and send the information of own node to all other hosts
            foreach (var host in listsOfHosts)
            {
                var toSendHost = AddNewHost(host.ToString());
                _proxy.Url = toSendHost.GetFullUrl();
                _proxy.AddNewHost(_nodeInfo.GetIpAndPort());
            }

            // Add ipAndPort of receiver
            AddNewHost(ipAndPort);

            Console.WriteLine("# Network connection success.");
        }

        /// <summary>
        /// Add new host to dictionary
        /// </summary>
        public NodeInfo AddNewHost(string ipAndPort)
        {
            var nodeInfo = new NodeInfo(ipAndPort);
            _hostLookup.Add(ipAndPort, new NodeInfo(ipAndPort));

            return nodeInfo;
        }
        #endregion

        #region Command: signoff

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
                _proxy.SignOff(myIp);
            }

            _hostLookup.Clear();
            LogHelper.WriteStatus("Sign off succeed.");
        }

        #endregion

        #region Command: start

        /// <summary>
        /// make all machines to wait the end of election.
        /// </summary>
        private void Start(bool isRicartAlgorithm)
        {
            if (_hostLookup.Count == 0)
            {
                Console.WriteLine("This client is not in network");
                return;
            }

            // 0. Send "Start" Message to All hosts and start StartProcess() thread
            foreach (NodeInfo host in _hostLookup.Values)
            {
                _proxy.Url = host.GetFullUrl();
                _proxy.GetStartMsg(isRicartAlgorithm);
            }
            var proc = new Thread(() => StartProcess(isRicartAlgorithm));
            proc.Start();

            Thread.Sleep(1000);

            // Start Election
            //LogHelper.WriteStatus("# Master Election Started #");
            ElectMasterNode();

        }

        #endregion

        #region Master election

        /// <summary>
        /// Start election of master node
        /// </summary>
        public void ElectMasterNode()
        {
            if (_hostLookup.Count == 0)
            {
                Console.WriteLine("This client is not in network");
                return;
            }

            // master node reset
            _masterNode = null;

            _electionAlgorithm.StartBullyElection(_hostLookup);
        }

        /// <summary>
        /// Start election of master node by receiving election message
        /// </summary>
        public void ElectMasterNodeByReceivingMsg(string id)
        {
            Console.WriteLine("## Received Election message from " + id);

            _electionAlgorithm.StartBullyElection(_hostLookup);
        }

        /// <summary>
        /// Setting master node. And starting algorithm if needed
        /// </summary>
        public void SetMasterNode(String ipAndPortMaster)
        {
            _masterNode = new NodeInfo(ipAndPortMaster);

            Console.WriteLine("## Master[" + _masterNode.GetIpAndPort() + "] is elected.");
            
            _electionAlgorithm.FinishElection();
            _isElectionFinished.Set();
        }

        #endregion

        #region Distributed Read and Write operation

        public void StartProcess(bool isRicartAlgorithm)
        {
            ResetVariables();

            LogHelper.WriteStatus("##### Start process using " + (isRicartAlgorithm ? "Ricart & Agrawala ": "Centralized ") 
                + "Algorithm.");
            LogHelper.WriteStatus("# 0. Waiting for the Master Election #");
            _isElectionFinished.Reset();
            
            // wait the end of master election - which will be done by other thread.
            _isElectionFinished.WaitOne();
            Console.WriteLine();
            LogHelper.WriteStatus("# Master Election ended. Now Start Read & Update Process.");
            Console.WriteLine();


            // 2. Read and Write operation using Centralized M.E. Sync. or Ricart Agrawala Sync.
            var rnd =
                new Random(int.Parse(Guid.NewGuid().ToString().Substring(0, 8),
                    System.Globalization.NumberStyles.HexNumber));
            //var hostList = _hostLookup.Values.ToList();
            int count = 0;
            _startTime = DateTime.Now;

            if (IsMasterNode())
            {
                return;
            }

            hostlistWithoutMaster = GetHostListWithoutMaster();
            printHostList(hostlistWithoutMaster);
            do
            {
                count++;
                Console.WriteLine("Start iteration #" + count);

                WaitRandomAmountTime(rnd);

                ProcessResourceFromMasterNode(isRicartAlgorithm);

                var executeTime = DateTime.Now;
                if (TimeSpan.Compare(executeTime - _startTime, _maxDuration) >= 0)
                {
                    if (isRicartAlgorithm && !IsMasterNode())
                    {
                        _ricartSyncAlgClient.Release_RA();
                    }
                    Console.WriteLine("Exited loop with " + (executeTime - _startTime));
                    break;
                }
            } while (true);

            Thread.Sleep(5000);
            var finalString = ReadFromMasterNode();
            Console.WriteLine("\n===============================");
            Console.WriteLine("Final string: " + finalString);
            Console.WriteLine("===============================");
            _isProcessFinished.Set();
        }

        /// <summary>
        /// Read, append and update
        /// </summary>
        private void ProcessResourceFromMasterNode(bool isRicartAlgorithm)
        {
            if (isRicartAlgorithm)
            {
                _ricartSyncAlgClient.SendSyncRequestToAllHosts_RA(hostlistWithoutMaster); //GetHostListWithoutMaster());
            }
            else
            {
                _centralizedSyncAlgClient.SendSyncRequestToMaster_CT(_masterNode);
            }

            // TEST:
            Thread.Sleep(2000);


            //initialize client to communicate with master node
            var readResFromMn = ReadFromMasterNode();

            //generate string
            var randomStr = GetRandomFruit();
            LogHelper.WriteStatus("Generated string: " + randomStr);

            //append string
            var appendedString = String.Concat(readResFromMn, randomStr);
            //add appended string to list
            _appendedStringSet.Add(appendedString);
            LogHelper.WriteStatus("Result of appended string " + appendedString);

            UpdateMasterNodeResource(appendedString);

            if (isRicartAlgorithm)
            {
                _ricartSyncAlgClient.Release_RA();
            }
            else
            {
                _centralizedSyncAlgClient.Release_CT();
            }
        }

        /// <summary>
        /// Reading from master node
        /// </summary>
        /// <returns></returns>
        private string ReadFromMasterNode()
        {
            LogHelper.WriteStatus("Reading resource from Master Node by host with IP: " + _nodeInfo.GetIpAndPort());
            
            if (_masterNode.Id == _nodeInfo.Id) throw new ArgumentException("Cannot read resource from same HOST!");

            string strResultFromMn;
            lock (Shared.SendLock)
            {
                _proxy.Url = _masterNode.GetFullUrl();
                strResultFromMn = _proxy.ReadResource(_nodeInfo.GetIpAndPort());
            }
            LogHelper.WriteStatus("Reading resource result " + strResultFromMn + " by host with IP: " + _nodeInfo.GetIpAndPort());
            return strResultFromMn;
        }

        private void WaitRandomAmountTime(Random rnd)
        {
            //wait random amount of time
            var sleepTime = rnd.Next(2000, 4001); //generate a random waiting time between 2s and 4s
            Console.WriteLine("Start waiting for " + sleepTime / 1000 + " sec");
            Thread.Sleep(sleepTime);
            Console.WriteLine("Finished waiting for " + sleepTime / 1000 + " sec");
        }

        /// <summary>
        /// write updated string to the master node
        /// </summary>
        /// <param name="appendedString"></param>
        private void UpdateMasterNodeResource(string appendedString)
        {
            if (_masterNode.Id == _nodeInfo.Id) throw new ArgumentException("Cannot read resource from same HOST!");
            lock (Shared.SendLock)
            {
                _proxy.Url = _masterNode.GetFullUrl();
                _proxy.UpdateResource(appendedString, _nodeInfo.GetIpAndPort());
            }
            LogHelper.WriteStatus("Updated string To Master Node with IP: " + _masterNode.GetIpAndPort());
        }

        private List<NodeInfo> GetHostListWithoutMaster()
        {
            var listCopy = _hostLookup.Values.ToList();
            //listCopy.Add(_nodeInfo);

            listCopy.RemoveAll(x => x.GetIpAndPort() == _masterNode.GetIpAndPort());
            return listCopy;
        }

        private void printHostList(List<NodeInfo> list)
        {
            Console.WriteLine("***** host list *****");
            foreach (var node in list)
            {
                Console.Write(node.Id + ", ");
            }
            Console.WriteLine();
        }

        public bool IsMasterNode()
        {
            return _nodeInfo.IsSameHost(_masterNode);
        }

        /// <summary>
        /// 
        /// </summary>
        private string GetRandomFruit()
        {
            var rnd = new Random(int.Parse(Guid.NewGuid().ToString().Substring(0, 8), System.Globalization.NumberStyles.HexNumber));

            string[] fruits = { "apple", "mango", "papaya", "banana", "guava", "pineapple" };
            return fruits[rnd.Next(0, fruits.Length)] + rnd.Next();
        }
        
        #endregion

        private void ResetVariables()
        {
            _appendedStringSet = new HashSet<string>();
            _masterNode = null;
            Resource = string.Empty;

            _electionAlgorithm.BullyReset();
            _centralizedSyncAlgClient.CentralizedReset();
            _ricartSyncAlgClient.RicartReset();
        }

    }
}
