using System.Messaging;

namespace MSMQCommander.ViewModels
{
    public class MessageGridRowViewModel
    {
        public string Id { get; private set; }
        public string Label { get; private set; }
        public string SentTime { get; private set; }

        public MessageGridRowViewModel(Message message)
        {
            Id = message.Id;
            Label = message.Label;
            SentTime = message.SentTime.ToShortDateString() + " " + message.SentTime.ToShortTimeString();
        }
    }
}