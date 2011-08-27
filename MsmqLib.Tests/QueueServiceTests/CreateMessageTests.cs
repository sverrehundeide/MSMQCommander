using System.Linq;
using MsmqLib.Tests.MessageObjectTestClasses;
using NUnit.Framework;

namespace MsmqLib.Tests.QueueServiceTests
{
    [TestFixture]
    public class CreateMessage_WhenValidParameters
        : ArrangeActAssertTestBase
    {
        private const string TestQueueName = "integrationTestQueue_1";
        private const string ComputerName = ".";
        private const string TestMessagesLabel = "CreateMessageTest";
        private QueueService _queueService;
        private string _queuePath;
        private TestClass1 _messageData;

        public override void Cleanup()
        {
            var queuePath = QueueTestHelper.CreateQueuePathForPrivateQueue(ComputerName, TestQueueName);
            new QueueService().ClearMessages(queuePath, TestMessagesLabel);
        }

        protected override void Arrange()
        {
            Cleanup();

            _queueService = new QueueService();
            _queuePath = QueueTestHelper.CreateQueuePathForPrivateQueue(ComputerName, TestQueueName);
            _queueService.CreateQueue(_queuePath);
            _messageData = new TestClass1 { IntValue1 = 1, StringValue1 = "string1" };
        }

        protected override void Act()
        {
            _queueService.CreateMessage(_queuePath, _messageData, TestMessagesLabel);
        }

        [Test]
        public void ShouldCreateMessage()
        {
            //Assert
            var messagesInQueue = _queueService.GetMessageInfos(_queuePath, TestMessagesLabel);
            Assert.That(messagesInQueue.Count(), Is.EqualTo(1));
        }
    }
}