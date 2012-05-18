using System.Messaging;

namespace MSMQCommander.Events
{
    public class QueueDeletedEvent
    {
        public MessageQueue MessageQueue { get; private set; }

        public QueueDeletedEvent(MessageQueue messageQueue)
        {
            MessageQueue = messageQueue;
        }
    }
}