using MsmqLib;

namespace MSMQCommander.ViewModels
{
    public class MessageGridRowViewModel
    {
        public string Label { get; private set; }
        public string SentTime { get; private set; }
        public string Id { get; private set; }

        public MessageGridRowViewModel(MessageInfo message)
        {
            Id = message.Id;
            Label = message.Label;
            SentTime = message.SentTime.ToString("yyyy-MM-dd HH:mm:ss.fff");
        }
    }
}