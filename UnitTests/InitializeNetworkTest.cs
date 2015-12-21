using System;
using System.Collections.Generic;
using DS_Network.Election;
using DS_Network.Helpers;
using DS_Network.Network;
using DS_Network.Sync.Ricart;
using NUnit.Framework;
using UnitTests.Mocks;

namespace UnitTests
{
    [TestFixture]
    //[SetUpFixture]
    public abstract class InitializeNetworkTest
    {
        public Host MasterHost;
        public List<Host> Hosts = new List<Host>();
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
                var electAlg = new Bully();
                var ipAddress = NetworkHelper.FindIp().ToString();
                var nodeInfo = new NodeInfo(ipAddress, port);
                var syncAlgorithm = new RicartSyncAlgorithm(nodeInfo.Id);

                var client = new Node(nodeInfo, mockProxy, electAlg, syncAlgorithm.Client, port); //client
                var server = new Server(port, syncAlgorithm.Server, client);
                var host = new Host(client, server);
                hostLookup.Add(nodeInfo.GetFullUrl(), host);

                Hosts.Add(host);
            }

            //var proxy2 = XmlRpcProxyGen.Create<IConnectionProxy>();
            var port2 = NetworkHelper.FindFreePort();
            var electAlg2 = new Bully();
            var ipAddress2 = NetworkHelper.FindIp().ToString();
            var nodeInfo2 = new NodeInfo(ipAddress2, port2);
            var syncAlgorithm2 = new RicartSyncAlgorithm(nodeInfo2.Id);
            var masterclient = new Node(nodeInfo2, mockProxy, electAlg2, syncAlgorithm2.Client, port2);
            MasterHost = new Host(masterclient,
                new Server(port2, syncAlgorithm2.Server, masterclient));
            hostLookup.Add(nodeInfo2.GetFullUrl(), MasterHost);
        }


    }
}
