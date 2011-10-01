using System.Collections.Generic;
using System.Linq;
using System.Messaging;
using Caliburn.Micro;
using MSMQCommander.Contex;
using MSMQCommander.Events;
using MsmqLib;

namespace MSMQCommander.ViewModels
{
    public class QueueTypeTreeNodeViewModel : PropertyChangedBase, IHandle<RefreshQueuesEvent>
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly IQueueService _queueService;
        private readonly string _computerName;
        private readonly string _queueType;

        public BindableCollection<QueueTreeNodeViewModel> Children { get; private set; }

        public QueueTypeTreeNodeViewModel(IEventAggregator eventAggregator, IQueueService queueService, QueueConnectionContext queueConnectionContext)
        {
            _eventAggregator = eventAggregator;
            _queueService = queueService;
            _computerName = queueConnectionContext.ComputerName;
            _queueType = "Private queues"; //TODO: Reuse class to support public queues

            IsExpanded = true;
            IsSelected = true;

            ReadAndInitializeChildQueues();

            _eventAggregator.Subscribe(this);
        }

        private void ReadAndInitializeChildQueues()
        {
            Children = new BindableCollection<QueueTreeNodeViewModel>();
            var privateQueues = _queueService.GetPrivateQueues(_computerName);
            foreach (var queue in privateQueues)
            {
                Children.Add(new QueueTreeNodeViewModel(_eventAggregator, queue, _queueService));
            }
        }

        public string Name
        {
            get { return _queueType; }
        }

        public bool IsExpanded { get; set; }
        public bool IsSelected { get; set; }

        public void Handle(RefreshQueuesEvent message)
        {
            var refreshedQueueList = _queueService.GetPrivateQueues(_computerName);
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
                Children.Add(new QueueTreeNodeViewModel(_eventAggregator, queue, _queueService));
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
    }
}