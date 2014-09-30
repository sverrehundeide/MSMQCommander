using System.Messaging;
using System.Windows;
using Caliburn.Micro;
using MSMQCommander.Dialogs;
using MSMQCommander.Events;
using MSMQCommander.Utils;
using MsmqLib;

namespace MSMQCommander.ViewModels
{
    public class QueueTreeNodeViewModel : 
        PropertyChangedBase,
        IHandle<RefreshQueuesEvent>,
        IHandle<QueueMessageCountChangedEvent>,
        IHandle<AutoRefreshEvent>
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly MessageQueue _messageQueue;
        private readonly IQueueService _queueService;
        private readonly IDialogService _dialogService;
        private bool _isExpanded;
        private bool _isSelected;

        public BindableCollection<JournalQueueTreeNodeViewModel> Children { get; private set; }

        public QueueTreeNodeViewModel(IEventAggregator eventAggregator, MessageQueue messageQueue, 
            IQueueService queueService, IDialogService dialogService)
        {
            _eventAggregator = eventAggregator;
            _messageQueue = messageQueue;
            _queueService = queueService;
            _dialogService = dialogService;

            ReadAndInitializeJournalQueue();

            eventAggregator.Subscribe(this);
        }

        private void ReadAndInitializeJournalQueue()
        {
            Children = new BindableCollection<JournalQueueTreeNodeViewModel>();
            var journalQueue = _queueService.GetJournalQueue(_messageQueue);
            Children.Add(new JournalQueueTreeNodeViewModel(_eventAggregator, journalQueue, _queueService, _dialogService));
        }

        public string Name
        {
            get
            {
                return _messageQueue.GetQueueNameExcludingQueueType();
            }
        }

        public string MessageCount
        {
            get
            {
                if (! _queueService.HasAccess(_messageQueue))
                {
                    return " (No access)";
                }

                return string.Format(" ({0})", _queueService.GetMessageCount(_messageQueue).ToString());
            }
        }

        public Visibility IsJournalingTogglingContextMenuVisible
        {
            get { return Visibility.Visible; }
        }

        public Visibility IsPurgeMessagesContextMenuVisible
        {
            get { return Visibility.Visible; }
        }

        public Visibility IsExportAllMessagesContextMenuVisible {
            get { return Visibility.Visible; }
        }

        public Visibility IsCreateNewQueueContextMenuVisible
        {
            get { return Visibility.Collapsed; }
        }

        public Visibility IsDeleteQueueContextMenuVisible
        {
            get { return Visibility.Visible; }
        }

        public bool IsExpanded
        {
            get { return _isExpanded; }
            set
            {
                if (value != _isExpanded)
                {
                    _isExpanded = value;
                    NotifyOfPropertyChange(() => IsExpanded);
                }
            }
        }

        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                if (value != _isSelected)
                {
                    _isSelected = value;
                    NotifyOfPropertyChange(() => IsSelected);
                    if (_isSelected)
                    {
                        _eventAggregator.Publish(new QueueSelectedEvent(_messageQueue));
                    }
                }
            }
        }

        public string ToggleJournalingCaption
        {
            get { return _messageQueue.UseJournalQueue ? "Disable journaling" : "Enable journaling"; }
        }

        public void ToggleJournaling()
        {
            _messageQueue.UseJournalQueue = !_messageQueue.UseJournalQueue;
            NotifyOfPropertyChange(() => ToggleJournalingCaption);
        }

        public bool Equals(MessageQueue x)
        {
            return _messageQueue.Path == x.Path;
        }

        public QueueClosedEvent CreateQueueClosedEvent()
        {
            return new QueueClosedEvent(_messageQueue);
        }

        public void Handle(RefreshQueuesEvent message)
        {
            Refresh();
        }

        public void Handle(QueueMessageCountChangedEvent message)
        {
            if (message.MessageQueue == _messageQueue)
            {
                NotifyOfPropertyChange(() => MessageCount);
            }
        }

        public void Handle(AutoRefreshEvent message)
        {
            NotifyOfPropertyChange(() => MessageCount);
            NotifyOfPropertyChange(() => Name);
        }

        public void PurgeMessages()
        {
            var question = string.Format("Delete all messages in the queue {0}?", Name);
            if (MessageBoxResult.Yes == _dialogService.AskQuestion(question, "Delete messages", MessageBoxButton.YesNo))
            {
                _queueService.PurgeMessages(_messageQueue);
                _eventAggregator.Publish(new RefreshQueuesEvent());
            }
        }

        public void ExportAllMessages()
        {
            if (_dialogService.ExportAllMessagesToQueue(_messageQueue.Path))
            {
                _eventAggregator.Publish(new RefreshQueuesEvent());
            }
        }

        public void CopyName()
        {
            Clipboard.SetText(this.Name);
        }

        public void DeleteQueue()
        {
            _dialogService.DeleteQueue(_messageQueue);
            _eventAggregator.Publish(new QueueDeletedEvent(_messageQueue));
        }
    }
}