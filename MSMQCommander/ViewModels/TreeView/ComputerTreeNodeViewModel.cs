using System.Windows;
using Caliburn.Micro;
using MSMQCommander.Contex;
using MSMQCommander.Events;

namespace MSMQCommander.ViewModels
{
    public class ComputerTreeNodeViewModel : 
        PropertyChangedBase,
        IHandle<QueueConnectionChangedEvent>
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly QueueConnectionContext _queueConnectionContext;

        public ComputerTreeNodeViewModel(IEventAggregator eventAggregator, QueueConnectionContext queueConnectionContext, QueueTypeTreeNodeViewModel queueTypeTreeNodeViewModel)
        {
            _eventAggregator = eventAggregator;
            _queueConnectionContext = queueConnectionContext;
            IsSelected = true;
            IsExpanded = true;

            Children = new BindableCollection<QueueTypeTreeNodeViewModel>
                           {
                               queueTypeTreeNodeViewModel,
                           };

            _eventAggregator.Subscribe(this);
        }

        public BindableCollection<QueueTypeTreeNodeViewModel> Children { get; private set; }

        public void Handle(QueueConnectionChangedEvent message)
        {
            NotifyOfPropertyChange(() => Name);
        }

        public string Name
        {
            get
            {
                if (_queueConnectionContext.ComputerName == ".")
                    return "localhost";

                return _queueConnectionContext.ComputerName;
            }
        }

        public bool IsSelected { get; set; }

        public bool IsExpanded { get; set; }

        public Visibility ContextMenuVisibility
        {
            get { return Visibility.Collapsed; }
        }
    }
}