using System.Messaging;
using Caliburn.Micro;
using MSMQCommander.Utils;

namespace MSMQCommander.ViewModels
{
    public class QueueTreeNodeViewModel : PropertyChangedBase
    {
        private readonly MessageQueue _messageQueue;

        public QueueTreeNodeViewModel(MessageQueue messageQueue)
        {
            _messageQueue = messageQueue;
        }

        public string Name
        {
            get { return _messageQueue.QueueNameExcludingQueueType(); }
        }
    }
}