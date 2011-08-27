using Caliburn.Micro;
using MSMQCommander.Contex;
using MsmqLib;

namespace MSMQCommander.ViewModels
{
    public class QueueTypeTreeNodeViewModel : PropertyChangedBase
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
        }

        private void ReadAndInitializeChildQueues()
        {
            Children = new BindableCollection<QueueTreeNodeViewModel>();
            var privateQueues = _queueService.GetPrivateQueues(_computerName);
            foreach (var queue in privateQueues)
            {
                Children.Add(new QueueTreeNodeViewModel(_eventAggregator, queue));
            }
        }

        public string Name
        {
            get { return _queueType; }
        }

        public bool IsExpanded { get; set; }
        public bool IsSelected { get; set; }
    }
}