using System.Collections.Generic;
using System.Linq;
using MsmqLib.Tests.MessageObjectTestClasses;
using NUnit.Framework;

namespace MsmqLib.Tests.QueueServiceTests
{
    [TestFixture(true)]
    [TestFixture(false)]
    public class GetMessageInfos_WhenHasMultipleMessages
        : ArrangeActAssertTestBase
    {
        private readonly bool _isTransactional;
        private const string TestQueueName = "integrationTestQueue_1";
        private const string ComputerName = ".";
        private const string TestMessagesLabel = "CreateMessageTest";
        const int MessageCount = 20;
        private QueueService _queueService;
        private string _queuePath;

        public GetMessageInfos_WhenHasMultipleMessages(bool isTransactional)
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