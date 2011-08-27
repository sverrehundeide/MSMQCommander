using System.Linq;
using System.Messaging;
using Caliburn.Micro;
using MSMQCommander.Contex;
using MsmqLib;

namespace MSMQCommander.ViewModels
{
    public class MessagesGridViewModel : PropertyChangedBase
    {
        private readonly IQueueService _queueService;
        private readonly MessageQueue _messageQueue;

        public BindableCollection<MessageGridRowViewModel> Messages { get; private set; }

        public MessagesGridViewModel(IQueueService queueService, CurrentSelectedQueueContext selectedQueueContext)
        {
            _queueService = queueService;
            _messageQueue = selectedQueueContext.CurrentSelectedMessageQueue;
            Messages = new BindableCollection<MessageGridRowViewModel>();
            RefreshMessages();
        }

        private void RefreshMessages()
        {
            var messageInfos = _queueService.GetMessageInfos(_messageQueue);
            Messages.Clear();
            Messages.AddRange(messageInfos.Select(info => new MessageGridRowViewModel(info)));
        }
    }
}