using System.Messaging;

namespace MSMQCommander.Events
{
    public class MessageSelectedEvent
    {
        public MessageQueue MessageQueue { get; private set; }
        public string MessageId { get; private set; }

        public MessageSelectedEvent(MessageQueue messageQueue, string messageId)
        {
            MessageQueue = messageQueue;
            MessageId = messageId;
        }
    }
}