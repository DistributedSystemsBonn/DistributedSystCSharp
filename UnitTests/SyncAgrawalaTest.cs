using System;
using NUnit.Framework;

namespace UnitTests
{
    [TestFixture]
    public class SyncAgrawalaTest : ElectionTest
    {
        [Test]
        public void TestAlgorithm()
        {
            JoinNetwork();
            Elect();

            foreach (var host in Hosts)
            {
                host.Client.StartAlgorithm();
            }


        }
    }
}
