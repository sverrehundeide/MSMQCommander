using System.Collections.Generic;
using System.Linq;
using MsmqLib.Tests.MessageObjectTestClasses;
using NUnit.Framework;

namespace MsmqLib.Tests.QueueServiceTests
{
    [TestFixture]
    public class GetMessageInfos_WhenHasMultipleMessages
        : ArrangeActAssertTestBase
    {
        private const string TestQueueName = "integrationTestQueue_1";
        private const string ComputerName = ".";
        private const string TestMessagesLabel = "CreateMessageTest";
        const int MessageCount = 10;
        private QueueService _queueService;
        private string _queuePath;

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

            for (int i = 1; i <= MessageCount; i++)
            {
                var messageData = new TestClass1 { IntValue1 = i, StringValue1 = "string" + i };
                _queueService.CreateMessage(_queuePath, messageData, TestMessagesLabel);
            }
        }

        protected override void Act()
        {
            Result = _queueService.GetMessageInfos(_queuePath, TestMessagesLabel);
        }

        [Test]
        public void ShouldReturnAllMessages()
        {
            Assert.That(GetResult<IEnumerable<MessageInfo>>().Count(), Is.EqualTo(MessageCount));
        }
    }
}