using System.Messaging;
using Caliburn.Micro;
using MSMQCommander.Contex;
using MSMQCommander.Utils;

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
                return _messageQueue.GetQueueNameIncludingQueueType();
            }
        }

        public bool Equals(MessageQueue queue)
        {
            return queue.Path == _messageQueue.Path;
        }
    }
}