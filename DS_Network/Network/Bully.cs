using System;
using System.Collections.Generic;
using System.Threading;

namespace DS_Network.Network
{
    class Bully
    {
        public static ManualResetEvent _isElectionFinished = new ManualResetEvent(false);
        public static ManualResetEvent _isThisNodeLost = new ManualResetEvent(false);
        public static bool _isExecuted = false;
        
        public Bully()
        {
        }

        /// <summary>
        /// reset static variables
        /// </summary>
        public void BullyReset()
        {
            _isElectionFinished.Reset();
            _isThisNodeLost.Reset();
            _isExecuted = false;
        }

        public void startBullyElection(NodeInfo node, Dictionary<String, NodeInfo> hostLookup, IConnectionProxy proxy)
        {
            if (_isExecuted)    // to assure single execution
                return;
            _isExecuted = true;

            int timeout = 2000;         // timeout for bully: 2 sec.
            Console.WriteLine(" - Master node election started.");
            
            foreach (NodeInfo host in hostLookup.Values)
            {
                if (node.Compare(host) == -1)    // find hosts with bigger id compared to the client
                {
                    Console.WriteLine("election target host: " + host.GetIpAndPort());
                    Thread send = new Thread(() => sendElectionMsg(node, host, proxy));
                    send.Start();
                }
            }
            if (_isThisNodeLost.WaitOne(timeout))
            {
                // signaled - lost
                Console.WriteLine("client lost.");
            }
            else
            {
                Console.WriteLine("Time out - client won");

                // TODO : Declare "I'm the new master" to all the nodes in the network
                foreach (NodeInfo host in hostLookup.Values)
                {
                    sendElectionFinalMsg(node, host, proxy);
                }
                sendElectionFinalMsg(node, node, proxy);    // set masternode itself
            }

            _isElectionFinished.WaitOne();  // wait for end of all of election processes
            Console.WriteLine(" - Master node election ended.");
            BullyReset();
        }

        private void sendElectionMsg(NodeInfo node, NodeInfo target, IConnectionProxy proxy)
        {
            proxy.Url = target.GetFullUrl();
            if (proxy.ReceiveElectionMsg(node.Id.ToString()))
            {
                // when target response
                _isThisNodeLost.Set();  // declare this node is lost
            }
        }

        private void sendElectionFinalMsg(NodeInfo node, NodeInfo target, IConnectionProxy proxy)
        {
            proxy.Url = target.GetFullUrl();
            proxy.SetMasterNode(node.GetIpAndPort());
        }

        public void finishElection()
        {
            _isElectionFinished.Set();
        }
    }
}
