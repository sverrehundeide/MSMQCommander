using System.Linq;
using System.Messaging;
using MsmqLib.Tests.MessageObjectTestClasses;
using NUnit.Framework;

namespace MsmqLib.Tests.QueueServiceTests
{
    [TestFixture(true)]
    [TestFixture(false)]
    public class DeleteMessage_WhenValidParameters
        : ArrangeActAssertTestBase
    {
        private readonly bool _isTransactional;
        private const string TestQueueName = "integrationTestQueue_1";
        private const string ComputerName = ".";
        private const string TestMessagesLabelForMessageWhichShouldNotBeDeleted = "Should not be deleted";
        private const string TestMessagesLabel = "CreateMessageTest";
        private QueueService _queueService;
        private string _queuePath;
        private TestClass1 _messageData;
        private MessageInfo _messageToDelete;

        public DeleteMessage_WhenValidParameters(bool isTransactional)
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

            _queueService.CreateMessage(_queuePath, new TestClass1 { StringValue1 = "Should not be deleted" }, TestMessagesLabelForMessageWhichShouldNotBeDeleted);

            _messageData = new TestClass1 { IntValue1 = 1, StringValue1 = "string1" };
            _queueService.CreateMessage(_queuePath, _messageData, TestMessagesLabel);
            
            _messageToDelete = _queueService.GetMessageInfos(_queuePath, TestMessagesLabel).Single();
        }

        protected override void Act()
        {
            string resultMessage;
            _queueService.DeleteMessage(new MessageQueue(_queuePath), _messageToDelete.Id, out resultMessage);
            Result = resultMessage;
        }

        [Test]
        public void ShouldDeleteMessageWithSpecifiedId()
        {
            var deletedMEssageExists = _queueService.GetMessageInfos(_queuePath, TestMessagesLabel).Any(m => m.Id == _messageToDelete.Id);
            Assert.That(deletedMEssageExists, Is.False);
        }

        [Test]
        public void ShouldKeepMessagesWithNonMatchingIds()
        {
            var messageWhichShouldNotBeDeleted = _queueService.GetMessageInfos(_queuePath, TestMessagesLabelForMessageWhichShouldNotBeDeleted).SingleOrDefault();
            Assert.That(messageWhichShouldNotBeDeleted, Is.Not.Null);
        }

        [Test]
        public void ShouldNotReturnErrorMessage()
        {
            Assert.That(Result, Is.Null);
        }
    }
    
    [TestFixture(true)]
    [TestFixture(false)]
    public class DeleteMessage_WhenMessageDoesNotExist
        : ArrangeActAssertTestBase
    {
        private readonly bool _isTransactional;
        private const string TestQueueName = "integrationTestQueue_1";
        private const string ComputerName = ".";
        private const string TestMessagesLabelForMessageWhichShouldNotBeDeleted = "Should not be deleted";
        private const string TestMessagesLabel = "CreateMessageTest";
        private QueueService _queueService;
        private string _queuePath;
        private TestClass1 _messageData;
        private MessageInfo _messageToDelete;

        public DeleteMessage_WhenMessageDoesNotExist(bool isTransactional)
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
        }

        protected override void Act()
        {
            const string nonExistingMessageId = "123";
            string resultMessage;
            _queueService.DeleteMessage(new MessageQueue(_queuePath), nonExistingMessageId, out resultMessage);
            Result = resultMessage;
        }

        [Test]
        public void ShouldReturnErrorMessage()
        {
            Assert.That(Result, Is.Not.Null);
        }
    }
}