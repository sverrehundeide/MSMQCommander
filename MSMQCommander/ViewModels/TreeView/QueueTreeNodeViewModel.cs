using System.Collections.Generic;
using System.Messaging;
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

        public QueueTreeNodeViewModel(IEventAggregator eventAggregator, MessageQueue messageQueue, IQueueService queueService)
        {
            _eventAggregator = eventAggregator;
            _messageQueue = messageQueue;
            _queueService = queueService;

            eventAggregator.Subscribe(this);
        }

        public string Name
        {
            get
            {
                return string.Format("{0} ({1})",
                    _messageQueue.QueueNameExcludingQueueType(), 
                    _queueService.GetMessageCount(_messageQueue));
            }
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
            NotifyOfPropertyChange(() => Name);
        }
    }
}