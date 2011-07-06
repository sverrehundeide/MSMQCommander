using System.Linq;
using System.Messaging;
using NUnit.Framework;

namespace MsmqLib.Tests.QueueServiceTests
{
    [TestFixture]
    public class CreateQueue_WhenNewQueue 
        : ArrangeActAssertTestBase
    {
        private const string NameOfNewQueue = "integrationTestQueue_1";
        private const string ComputerName = ".";

        public override void Cleanup()
        {
            QueueTestHelper.CleanupPrivateTestQueues(ComputerName, NameOfNewQueue);
        }

        protected override void Arrange()
        {
            Cleanup(); //Make sure queue does not exist
        }

        protected override void Act()
        {
            QueueTestHelper.CreatePrivateQueue(ComputerName, NameOfNewQueue);
        }

        [Test]
        public void ShouldCreateQueue()
        {
            var result = new QueueService().GetPrivateQueues(ComputerName);
            Assert.That(result
                .Where(q => q.QueueName.ToLower().Contains(NameOfNewQueue.ToLower()))
                .Count(),
                Is.EqualTo(1));
        }
    }

    [TestFixture]
    public class CreateQueue_WhenQueueExists
        : ArrangeActAssertTestBase
    {
        private const string NameOfNewQueue = "integrationTestQueue_1";
        private const string ComputerName = ".";

        public override void Cleanup()
        {
            QueueTestHelper.CleanupPrivateTestQueues(ComputerName, NameOfNewQueue);
        }

        protected override void Arrange()
        {
            Cleanup(); //Make sure queue does not exist
            QueueTestHelper.CreatePrivateQueue(ComputerName, NameOfNewQueue);
        }

        protected override void Act()
        {
            QueueTestHelper.CreatePrivateQueue(ComputerName, NameOfNewQueue);
        }

        [Test]
        public void ShouldReuseExistingQueue()
        {
            var result = new QueueService().GetPrivateQueues(ComputerName);
            Assert.That(result
                .Where(q => q.QueueName.ToLower().Contains(NameOfNewQueue.ToLower()))
                .Count(),
                Is.EqualTo(1));
        }
    }

    [TestFixture]
    public class GetPrivateQueues_WhenLocalhostAndHasQueues
        : ArrangeActAssertTestBase
    {
        private const string TestQueuesPrefix = "integrationTestQueue_";
        private const string ComputerName = ".";

        public override void Cleanup()
        {
            QueueTestHelper.CleanupPrivateTestQueues(ComputerName, TestQueuesPrefix);
        }

        protected override void  Arrange()
        {
            QueueTestHelper.CreatePrivateQueue(ComputerName, TestQueuesPrefix, 1);
            QueueTestHelper.CreatePrivateQueue(ComputerName, TestQueuesPrefix, 2);
        }

        protected override void Act()
        {
            Result = new QueueService().GetPrivateQueues(ComputerName);
        }

        [Test]
        public void ShouldReturnQueues()
        {
            Assert.That(GetResult<MessageQueue[]>()
                .Where(q => q.QueueName.ToLower().Contains(TestQueuesPrefix.ToLower()))
                .Distinct()
                .Count(), 
                Is.EqualTo(2));
        }

        [Test]
        public void ReturnedQueuesShouldBePrivate()
        {
            Assert.That(GetResult<MessageQueue[]>()
                .Where(q => false == q.Path.ToLower().Contains(@"private$"))
                .Count(),
                Is.EqualTo(0));
        }
    }
}