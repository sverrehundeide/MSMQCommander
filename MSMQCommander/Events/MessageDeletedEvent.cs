namespace MSMQCommander.Events
{
    public class MessageDeletedEvent
    {
        public string MessageId { get; private set; }

        public MessageDeletedEvent(string messageId)
        {
            MessageId = messageId;
        }
    }
}
