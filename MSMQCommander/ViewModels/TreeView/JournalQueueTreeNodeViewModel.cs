using System.Messaging;
using System.Windows;
using Caliburn.Micro;
using MSMQCommander.Events;
using MsmqLib;

namespace MSMQCommander.ViewModels
{
    public class JournalQueueTreeNodeViewModel :
        PropertyChangedBase,
        IHandle<RefreshQueuesEvent>
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly MessageQueue _journalQueue;
        private readonly IQueueService _queueService;
        private bool _isSelected;

        public JournalQueueTreeNodeViewModel(IEventAggregator eventAggregator, MessageQueue journalQueue, IQueueService queueService)
        {
            _eventAggregator = eventAggregator;
            _journalQueue = journalQueue;
            _queueService = queueService;

            _eventAggregator.Subscribe(this);
        }

        public string Name
        {
            get { return "Journal"; }
        }

        public string MessageCount
        {
            get { return string.Format(" ({0})", _queueService.GetMessageCount(_journalQueue)); }
        }

        public Visibility IsJournalingTogglingContextMenuVisible
        {
            get { return Visibility.Collapsed; }
        }

        public Visibility IsPurgeMessagesContextMenuVisible
        {
            get { return Visibility.Visible; }
        }

        public Visibility IsCreateNewQueueContextMenuVisible
        {
            get { return Visibility.Collapsed; }
        }

        public Visibility IsDeleteQueueContextMenuVisible
        {
            get { return Visibility.Collapsed; }
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
                        _eventAggregator.Publish(new QueueSelectedEvent(_journalQueue));
                    }
                }
            }
        }

        public void Handle(RefreshQueuesEvent message)
        {
            Refresh();
        }
    }
}