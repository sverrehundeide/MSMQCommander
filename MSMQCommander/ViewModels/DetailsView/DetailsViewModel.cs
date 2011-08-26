using System.Messaging;
using Caliburn.Micro;
using MSMQCommander.Contex;

namespace MSMQCommander.ViewModels
{
    public class DetailsViewModel : PropertyChangedBase
    {
        private readonly MessageQueue _messageQueue;

        public DetailsViewModel(CurrentSelectedQueueContext currentSelectedQueueContext)
        {
            _messageQueue = currentSelectedQueueContext.CurrentSelectedMessageQueue;
        }

        public string Title
        {
            get
            {
                if (_messageQueue == null)
                    return string.Empty;
                return _messageQueue.QueueName;
            }
        }
    }
}