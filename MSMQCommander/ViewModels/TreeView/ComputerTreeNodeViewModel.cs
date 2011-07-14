using Caliburn.Micro;

namespace MSMQCommander.ViewModels
{
    public class ComputerTreeNodeViewModel : PropertyChangedBase
    {
        private readonly IEventAggregator _eventAggregator;
        private string _name;

        public ComputerTreeNodeViewModel(IEventAggregator eventAggregator, string name)
        {
            _eventAggregator = eventAggregator;
            IsSelected = true;
            IsExpanded = true;

            Name = name;
            Children = new BindableCollection<QueueTypeTreeNodeViewModel>
                           {
                               new QueueTypeTreeNodeViewModel(_eventAggregator, Name, "Private queues")
                           };
        }

        public BindableCollection<QueueTypeTreeNodeViewModel> Children { get; private set; } 

        public string Name
        {
            get
            {
                if (_name == ".")
                    return "localhost";

                return _name;
            }
            private set
            {
                _name = value;
                NotifyOfPropertyChange(() => Name);
            }
        }

        public bool IsSelected { get; set; }
        public bool IsExpanded { get; set; }
    }
}