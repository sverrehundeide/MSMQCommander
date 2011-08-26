using System.Messaging;

namespace MSMQCommander.Contex
{
    public class CurrentSelectedQueueContext
    {
        public MessageQueue CurrentSelectedMessageQueue { get; set; }
    }
}