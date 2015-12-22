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
                //Assert.IsTrue(masterNodeId == host.Client.MasterNode.Id);
            }

            //Assert.IsTrue(MasterHost.Client.IsMasterNode());
        }
    }
}
