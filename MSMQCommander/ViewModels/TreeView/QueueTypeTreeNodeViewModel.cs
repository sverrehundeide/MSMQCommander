using Caliburn.Micro;

namespace MSMQCommander.ViewModels
{
    public class QueueTypeTreeNodeViewModel : PropertyChangedBase
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly string _computerName;
        private readonly string _queueType;

        public BindableCollection<QueueTreeNodeViewModel> Children { get; private set; }

        public QueueTypeTreeNodeViewModel(IEventAggregator eventAggregator, string computerName, string queueType)
        {
            _eventAggregator = eventAggregator;
            _computerName = computerName;
            _queueType = queueType;

            IsExpanded = true;
            IsSelected = true;

            ReadAndInitializeChildQueues();
        }

        private void ReadAndInitializeChildQueues()
        {
            Children = new BindableCollection<QueueTreeNodeViewModel>();
            var privateQueues = new MsmqLib.QueueService().GetPrivateQueues(_computerName);
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