using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using CookComputing.XmlRpc;
using DS_Network.Network;

namespace DS_Network.Election
{
    public class Bully : IElectionAlgorithm
    {
        private ManualResetEvent _isElectionFinished = new ManualResetEvent(false);
        private ManualResetEvent _isThisNodeLost = new ManualResetEvent(false);
        private NodeInfo _node;
        private IConnectionProxy _proxy;
        private bool _isExecuted;

        public Bully(NodeInfo node, IConnectionProxy proxy)
        {
            _node = node;
            _proxy = proxy;
            _isExecuted = false;
        }

        /// <summary>
        /// reset variables
        /// </summary>
        public void BullyReset()
        {
            _isElectionFinished.Reset();
            _isThisNodeLost.Reset();
            _isExecuted = false;
        }


        /// <summary>
        /// First step of the bully algorithm
        /// </summary>
        public void StartBullyElection(Dictionary<string, NodeInfo> hostLookup)
        {
            if (_isExecuted) return;
            _isExecuted = true;
            
            int timeout = 2000; // timeout for bully: 2 sec.
            Console.WriteLine("## Master node election started. ##");

            // find hosts with bigger id compared to the client
            foreach (var host in hostLookup.Values.Where(host => _node.Compare(host) == -1))
            {
                Console.WriteLine("election target host: " + host.GetIpAndPort());

                var hostCopy = host;
                var send = new Thread(() => SendElectionMsg(hostCopy, _proxy));
                send.Start();
            }
            if (_isThisNodeLost.WaitOne(timeout))
            {
                // signaled - lost
                Console.WriteLine("client lost.");
            }
            else
            {
                Console.WriteLine("Time out - client won");

                // Declare "I'm the new master" to all the nodes in the network
                // When this node is the winner(master), it send final election msg to other machines
                foreach (var host in hostLookup.Values)
                {
                    SendElectionFinalMsg(host);
                }
                SendElectionFinalMsg(_node); // set masternode itself
            }

            _isElectionFinished.WaitOne(); // wait for end of all of election processes
            Console.WriteLine(" - Master node election ended.");
            BullyReset();
        }

        /// <summary>
        /// Send election message to target host
        /// </summary>
        public void SendElectionMsg(NodeInfo target, IConnectionProxy proxy)
        {
            //var proxy1 = XmlRpcProxyGen.Create<IConnectionProxy>();
            proxy.Url = target.GetFullUrl();
            if (proxy.ReceiveElectionMsg(_node.Id.ToString()))
            {
                // when target response
                _isThisNodeLost.Set();  // declare this node is lost
            }
        }

        /// <summary>
        /// When this node is the winner(master), it send final election msg to other machines
        /// </summary>
        public void SendElectionFinalMsg(NodeInfo target)
        {
            //var proxy1 = XmlRpcProxyGen.Create<IConnectionProxy>();
            _proxy.Url = target.GetFullUrl();
            _proxy.SetMasterNode(_node.GetIpAndPort());
        }


        /// <summary>
        /// 
        /// </summary>
        public void FinishElection()
        {
            _isElectionFinished.Set();
        }
    }
}
