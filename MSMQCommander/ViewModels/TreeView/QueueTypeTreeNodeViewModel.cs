using Caliburn.Micro;

namespace MSMQCommander.ViewModels
{
    public class QueueTypeTreeNodeViewModel : PropertyChangedBase
    {
        private readonly string _computerName;
        private readonly string _queueType;

        public BindableCollection<QueueTreeNodeViewModel> Children { get; private set; } 

        public QueueTypeTreeNodeViewModel(string computerName, string queueType)
        {
            _computerName = computerName;
            _queueType = queueType;
            ReadAndInitializeChildQueues();
        }

        private void ReadAndInitializeChildQueues()
        {
            Children = new BindableCollection<QueueTreeNodeViewModel>();
            var privateQueues = new MsmqLib.QueueService().GetPrivateQueues(_computerName);
            foreach (var queue in privateQueues)
            {
                Children.Add(new QueueTreeNodeViewModel(queue));
            }
        }

        public string Name
        {
            get { return _queueType; }
        }
    }
}