using System.Linq;
using System.Messaging;
using Caliburn.Micro;
using MSMQCommander.Contex;
using MSMQCommander.Dialogs;
using MSMQCommander.Events;
using MsmqLib;

namespace MSMQCommander.ViewModels
{
    public class MessagesGridViewModel : PropertyChangedBase, IHandle<RefreshQueuesEvent>
    {
        private readonly IQueueService _queueService;
        private readonly IEventAggregator _eventAggregator;
        private readonly IDialogService _dialogService;
        private readonly MessageQueue _messageQueue;
        private MessageGridRowViewModel _lastSelectedItem;

        public BindableCollection<MessageGridRowViewModel> Messages { get; private set; }

        public MessagesGridViewModel(IQueueService queueService, IEventAggregator eventAggregator,
            CurrentSelectedQueueContext selectedQueueContext, IDialogService dialogService)
        {
            _queueService = queueService;
            _eventAggregator = eventAggregator;
            _dialogService = dialogService;
            _messageQueue = selectedQueueContext.CurrentSelectedMessageQueue;
            Messages = new BindableCollection<MessageGridRowViewModel>();
            RefreshMessages();
            _eventAggregator.Subscribe(this);
        }

        private void RefreshMessages()
        {
            var messageInfos = _queueService.GetMessageInfos(_messageQueue);
            Messages.Clear();
            Messages.AddRange(messageInfos.Select(info => new MessageGridRowViewModel(info)));
        }

        public MessageGridRowViewModel SelectedItem
        {
            set
            {
                _lastSelectedItem = value;
                NotifyOfPropertyChange(() => IsExportMessageBodyEnabled);

                if (value == null)
                    return;

                _eventAggregator.Publish(new MessageSelectedEvent(_messageQueue, value.Id));
            }
        }

        public void Handle(RefreshQueuesEvent message)
        {
            RefreshMessages();
        }

        public bool IsExportMessageBodyEnabled
        {
            get { return _lastSelectedItem != null; }
        }

        public void ExportMessageBody()
        {
            _dialogService.ExportMessageBody(_messageQueue, _lastSelectedItem.Id);
        }
    }
}