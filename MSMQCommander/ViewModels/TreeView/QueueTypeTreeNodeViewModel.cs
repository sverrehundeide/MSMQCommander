using System.Collections.Generic;
using System.Linq;
using System.Messaging;
using System.Windows;
using Caliburn.Micro;
using MSMQCommander.Contex;
using MSMQCommander.Dialogs;
using MSMQCommander.Events;
using MsmqLib;

namespace MSMQCommander.ViewModels
{
    public class QueueTypeTreeNodeViewModel : 
        PropertyChangedBase,
        IHandle<QueueConnectionChangedEvent>,
        IHandle<RefreshQueuesEvent>,
        IHandle<QueueDeletedEvent>
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly IQueueService _queueService;
        private readonly QueueConnectionContext _queueConnectionContext;
        private readonly IDialogService _dialogService;
        private readonly string _queueType;

        public BindableCollection<QueueTreeNodeViewModel> Children { get; private set; }

        public QueueTypeTreeNodeViewModel(IEventAggregator eventAggregator, IQueueService queueService, 
            QueueConnectionContext queueConnectionContext, IDialogService dialogService)
        {
            _eventAggregator = eventAggregator;
            _queueService = queueService;
            _queueConnectionContext = queueConnectionContext;
            _dialogService = dialogService;
            _queueType = "Private queues"; //TODO: Reuse class to support public queues

            IsExpanded = true;
            IsSelected = true;

            ReadAndInitializeChildQueues();

            _eventAggregator.Subscribe(this);
        }

        private void ReadAndInitializeChildQueues()
        {
            Children = new BindableCollection<QueueTreeNodeViewModel>();
            var privateQueues = _queueService.GetPrivateQueues(_queueConnectionContext.ComputerName);
            foreach (var queue in privateQueues)
            {
                Children.Add(new QueueTreeNodeViewModel(_eventAggregator, queue, _queueService, _dialogService));
            }
        }

        public string Name
        {
            get { return _queueType; }
        }

        public bool IsExpanded { get; set; }
        public bool IsSelected { get; set; }

        public void Handle(QueueConnectionChangedEvent queueConnectionChangedEvent)
        {
            RefreshQueues();
        }

        public void Handle(RefreshQueuesEvent message)
        {
            RefreshQueues();
        }

        public void Handle(QueueDeletedEvent message)
        {
            RefreshQueues();
        }

        private void RefreshQueues()
        {
            var refreshedQueueList = _queueService.GetPrivateQueues(_queueConnectionContext.ComputerName);
            AddNewQueues(refreshedQueueList);
            RemoveDeletedQueues(refreshedQueueList);
        }

        private void AddNewQueues(IEnumerable<MessageQueue> refreshedQueueList)
        {
            var newQueuesAdded = false;
            foreach (var queue in refreshedQueueList)
            {
                if (Children.Any(existingChildQueue => existingChildQueue.Equals(queue)))
                {
                    continue;
                }
                Children.Add(new QueueTreeNodeViewModel(_eventAggregator, queue, _queueService, _dialogService));
                newQueuesAdded = true;
            }
            if (newQueuesAdded)
            {
                NotifyOfPropertyChange(() => Children);
            }
        }

        private void RemoveDeletedQueues(MessageQueue[] refreshedQueueList)
        {
            var nodesToRemoveFromTree = new List<QueueTreeNodeViewModel>();
            foreach (var existingChildQueue in Children)
            {
                if (refreshedQueueList.Any(refreshedQueue => existingChildQueue.Equals(refreshedQueue)))
                {
                    continue;
                }
                _eventAggregator.Publish(existingChildQueue.CreateQueueClosedEvent());
                nodesToRemoveFromTree.Add(existingChildQueue);
            }
            if (nodesToRemoveFromTree.Any())
            {
                Children.RemoveRange(nodesToRemoveFromTree);
                NotifyOfPropertyChange(() => Children);
            }
        }

        public void CreateNewQueue()
        {
            _dialogService.CreateNewQueue();
            RefreshQueues();
        }

        public Visibility ContextMenuVisibility
        {
            get { return Visibility.Visible; }
        }

        public Visibility IsCreateNewQueueContextMenuVisible
        {
            get { return Visibility.Visible; }
        }

        public Visibility IsJournalingTogglingContextMenuVisible
        {
            get { return Visibility.Collapsed; }
        }

        public Visibility IsPurgeMessagesContextMenuVisible
        {
            get { return Visibility.Collapsed; }
        }

        public Visibility IsDeleteQueueContextMenuVisible
        {
            get {return Visibility.Collapsed; }
        }
    }
}