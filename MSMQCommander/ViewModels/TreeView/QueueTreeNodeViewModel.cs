using System.Messaging;
using System.Windows;
using Caliburn.Micro;
using MSMQCommander.Events;
using MSMQCommander.Utils;
using MsmqLib;

namespace MSMQCommander.ViewModels
{
    public class QueueTreeNodeViewModel : 
        PropertyChangedBase,
        IHandle<RefreshQueuesEvent>
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly MessageQueue _messageQueue;
        private readonly IQueueService _queueService;
        private bool _isExpanded;
        private bool _isSelected;

        public BindableCollection<JournalQueueTreeNodeViewModel> Children { get; private set; }

        public QueueTreeNodeViewModel(IEventAggregator eventAggregator, MessageQueue messageQueue, IQueueService queueService)
        {
            _eventAggregator = eventAggregator;
            _messageQueue = messageQueue;
            _queueService = queueService;

            ReadAndInitializeJournalQueue();

            eventAggregator.Subscribe(this);
        }

        private void ReadAndInitializeJournalQueue()
        {
            Children = new BindableCollection<JournalQueueTreeNodeViewModel>();
            var journalQueue = _queueService.GetJournalQueue(_messageQueue);
            Children.Add(new JournalQueueTreeNodeViewModel(_eventAggregator, journalQueue, _queueService));
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
            get { return string.Format(" ({0})", _queueService.GetMessageCount(_messageQueue).ToString()); }
        }


        public Visibility IsJournalingTogglingContextMenuVisible
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

        public bool IsJournalingEnabled
        {
            get { return _messageQueue.UseJournalQueue; }
            set { _messageQueue.UseJournalQueue = value; }
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
    }
}