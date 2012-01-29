using System.Messaging;

namespace MSMQCommander.Events
{
    public class QueueMessageCountChangedEvent
    {
        public MessageQueue MessageQueue { get; set; }

        public QueueMessageCountChangedEvent(MessageQueue messageQueue)
        {
            MessageQueue = messageQueue;
        }
    }
}
