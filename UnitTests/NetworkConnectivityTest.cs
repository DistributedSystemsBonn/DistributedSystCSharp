using System;
using NUnit.Framework;

namespace UnitTests
{
    [TestFixture]
    public class NetworkConnectivityTest : InitializeNetworkTest
    {
        /// <summary>
        /// Connnect to master node and check gethosts count and ips
        /// </summary> 
        [Test]
        public void JoinNetwork()
        {
            var masterHostId = MasterHost.Client.NodeInfo.GetIpAndPort();

            foreach (var host in Hosts)
            {
                host.Client.Join(masterHostId);
            }

            Assert.That(MasterHost.Client.HostLookup.Count == HostNumber);
        }
    }
}
