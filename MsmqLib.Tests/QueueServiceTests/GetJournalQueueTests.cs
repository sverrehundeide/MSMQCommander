using System.Linq;
using System.Messaging;
using NUnit.Framework;

namespace MsmqLib.Tests.QueueServiceTests
{
    [TestFixture]
    public class GetJournalQueue_WhenNewQueue
        : ArrangeActAssertTestBase
    {
        private MessageQueue _journalQueueParent;
        private const string NameOfJournalQueueParent = "integrationTestQueue_JournalTest";
        private const string ComputerName = ".";

        public override void Cleanup()
        {
            QueueTestHelper.DeletePrivateQueueIfExists(ComputerName, NameOfJournalQueueParent);
        }

        protected override void Arrange()
        {
            Cleanup();
            _journalQueueParent = QueueTestHelper.CreatePrivateQueue(ComputerName, NameOfJournalQueueParent);
        }

        protected override void Act()
        {
            Result = new QueueService().GetJournalQueue(_journalQueueParent);
        }

        [Test]
        public void ShouldCreateQueue()
        {
            var expected = QueueTestHelper.CreateQueuePathForPrivateQueue(ComputerName, NameOfJournalQueueParent) + ";JOURNAL";
            Assert.That(GetResult<MessageQueue>().Path, Is.EqualTo(expected));
        }
    }
}