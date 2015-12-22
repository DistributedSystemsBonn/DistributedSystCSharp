using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
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

            var taskList = new List<Task>();
            foreach (var host in Hosts)
            {
                var newSyncTask = Task.Factory.StartNew(() => host.Client.StartAlgorithm());
                taskList.Add(newSyncTask);
                //newSyncTask.Start();
            }
            Task.WaitAll(taskList.ToArray());
            Console.WriteLine(MasterHost.Client.Resource);
            Assert.IsTrue(true);
        }
    }
}
