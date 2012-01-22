using System.Messaging;
using MsmqLib.Tests.MessageObjectTestClasses;
using NUnit.Framework;

namespace MsmqLib.Tests.QueueServiceTests
{
    [TestFixture(true)]
    [TestFixture(false)]
    public class GetMessageCount_WhenHasMessages
        : ArrangeActAssertTestBase
    {
        private readonly bool _isTransactional;
        private const string TestQueueName = "integrationTestQueue_MessageCount";
        private const string ComputerName = ".";
        private const string TestMessagesLabel = "MessageCountTest";
        const int MessageCount = 20;
        private QueueService _queueService;
        private MessageQueue _messageQueue;
        private string _queuePath;

        public GetMessageCount_WhenHasMessages(bool isTransactional)
        {
            _isTransactional = isTransactional;
        }

        public override void Cleanup()
        {
            QueueTestHelper.DeletePrivateQueueIfExists(ComputerName, TestQueueName, _isTransactional);
        }

        protected override void Arrange()
        {
            Cleanup();

            _queueService = new QueueService();
            _queuePath = QueueTestHelper.CreateQueuePathForPrivateQueue(ComputerName, TestQueueName, _isTransactional);
            _queueService.CreateQueue(_queuePath, _isTransactional);

            for (int i = 1; i <= MessageCount; i++)
            {
                var messageData = new TestClass1 { IntValue1 = i, StringValue1 = "string" + i };
                _queueService.CreateMessage(_queuePath, messageData, TestMessagesLabel);
            }

            _messageQueue = new MessageQueue(_queuePath);
        }

        protected override void Act()
        {
            Result = _queueService.GetMessageCount(_messageQueue);
        }

        [Test]
        public void ShouldReturnCorrectMessageCount()
        {
            Assert.That(Result, Is.EqualTo(MessageCount));
        }
    }
}
