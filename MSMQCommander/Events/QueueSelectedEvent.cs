using System.Messaging;

namespace MSMQCommander.Events
{
    public class QueueSelectedEvent
    {
        public MessageQueue MessageQueue { get; private set; }

        public QueueSelectedEvent(MessageQueue messageQueue)
        {
            MessageQueue = messageQueue;
        }
    }
}