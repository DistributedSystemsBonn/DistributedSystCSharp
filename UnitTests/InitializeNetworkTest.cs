using System;
using System.Collections.Generic;
using DS_Network.Election;
using DS_Network.Helpers;
using DS_Network.Network;
using DS_Network.Sync.Centralized;
using DS_Network.Sync.Ricart;
using NUnit.Framework;
using UnitTests.Mocks;

namespace UnitTests
{
    //[TestFixture]
    [SetUpFixture]
    public abstract class InitializeNetworkTest
    {
        public static Host MasterHost;
        public static List<Host> Hosts = new List<Host>();
        public int HostNumber = 4;

        private static bool initialized = false;

        [SetUp]
        //[OneTimeSetUp]
        public virtual void Init()
        {
            //if (initialized) Assert.Fail("fixture setup called multiple times");
            if (initialized) return;

            initialized = true;

            var hostLookup = new Dictionary<String, Host>();
            var mockProxy = new ConnectionProxyMock(hostLookup);

            for (var i = 0; i < HostNumber; i++)
            {
                //var proxy = XmlRpcProxyGen.Create<IConnectionProxy>();
                
                var port = NetworkHelper.FindFreePort();
                var ipAddress = NetworkHelper.FindIp().ToString();
                var nodeInfo = new NodeInfo(ipAddress, port);
                var electAlg = new Bully(nodeInfo, mockProxy);
                var ricartSyncAlgorithm = new RicartSyncAlgorithm(nodeInfo, mockProxy);
                var centralizedSyncAlgorithm = new CentralizedSyncAlgorithm(nodeInfo, mockProxy);

                var client = new Node(nodeInfo, mockProxy, electAlg, ricartSyncAlgorithm.Client, centralizedSyncAlgorithm.Client);
                var server = new Server(port, client, ricartSyncAlgorithm.Server, centralizedSyncAlgorithm.Server);
                var host = new Host(client, server);
                hostLookup.Add(nodeInfo.GetFullUrl(), host);

                Hosts.Add(host);
            }

            //var proxy2 = XmlRpcProxyGen.Create<IConnectionProxy>();
            var port2 = NetworkHelper.FindFreePort();
            var ipAddress2 = "255.255.255.255";
            var nodeInfo2 = new NodeInfo(ipAddress2, port2);
            var electAlg2 = new Bully(nodeInfo2, mockProxy);
            var ricartSyncAlgorithm2 = new RicartSyncAlgorithm(nodeInfo2, mockProxy);
            var centralizedSyncAlgorithm2 = new CentralizedSyncAlgorithm(nodeInfo2, mockProxy);

            var masterclient = new Node(nodeInfo2, mockProxy, electAlg2, ricartSyncAlgorithm2.Client, centralizedSyncAlgorithm2.Client);
            MasterHost = new Host(masterclient,
                new Server(port2, masterclient, ricartSyncAlgorithm2.Server, centralizedSyncAlgorithm2.Server));
            hostLookup.Add(nodeInfo2.GetFullUrl(), MasterHost);
        }


    }
}
