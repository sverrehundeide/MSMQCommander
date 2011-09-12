using System.Messaging;

namespace MSMQCommander.Events
{
    public class QueueClosedEvent
    {
        public MessageQueue MessageQueue { get; private set; }

        public QueueClosedEvent(MessageQueue messageQueue)
        {
            MessageQueue = messageQueue;
        }
    }
}