using System.Messaging;
using NUnit.Framework;

namespace MsmqLib.Tests.QueueServiceTests
{
    [TestFixture(true)]
    [TestFixture(false)]
    public class GetJournalQueue_WhenNewQueue
        : ArrangeActAssertTestBase
    {
        private readonly bool _isTransactional;
        private MessageQueue _journalQueueParent;
        private const string NameOfJournalQueueParent = "integrationTestQueue_JournalTest";
        private const string ComputerName = ".";

        public GetJournalQueue_WhenNewQueue(bool isTransactional)
        {
            _isTransactional = isTransactional;
        }

        public override void Cleanup()
        {
            QueueTestHelper.DeletePrivateQueueIfExists(ComputerName, NameOfJournalQueueParent, _isTransactional);
        }

        protected override void Arrange()
        {
            Cleanup();
            _journalQueueParent = QueueTestHelper.CreatePrivateQueue(ComputerName, NameOfJournalQueueParent, _isTransactional);
        }

        protected override void Act()
        {
            Result = new QueueService().GetJournalQueue(_journalQueueParent);
        }

        [Test]
        public void ShouldCreateQueue()
        {
            var expected = QueueTestHelper.CreateQueuePathForPrivateQueue(ComputerName, NameOfJournalQueueParent, _isTransactional) + ";JOURNAL";
            Assert.That(GetResult<MessageQueue>().Path, Is.EqualTo(expected));
        }
    }
}