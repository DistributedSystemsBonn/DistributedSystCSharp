using System;
using System.Linq;
using NUnit.Framework;

namespace UnitTests
{
    [TestFixture]
    public class ElectionTest : NetworkConnectivityTest
    {
        [Test]
        public void Elect()
        {
            //Init();
            //JoinNetwork();

            Hosts.First().Client.ElectMasterNode();

            var masterNodeId = MasterHost.Client.NodeInfo.Id;

            foreach (var host in Hosts)
            {
                var id = host.Client.MasterNode.Id;
                Assert.IsTrue(id == masterNodeId);
            }

            //Assert.IsTrue(MasterHost.Client.IsMasterNode());
        }
    }
}
