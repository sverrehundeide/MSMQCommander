using Caliburn.Micro;

namespace MSMQCommander.ViewModels
{
    public class ComputerTreeNodeViewModel : PropertyChangedBase
    {
        private string _name;

        public ComputerTreeNodeViewModel(string name)
        {
            Name = name;
            Children = new BindableCollection<QueueTypeTreeNodeViewModel>{new QueueTypeTreeNodeViewModel("Private queues")};
        }

        public BindableCollection<QueueTypeTreeNodeViewModel> Children { get; private set; } 

        public string Name
        {
            get { return _name; }
            private set
            {
                _name = value;
                NotifyOfPropertyChange(() => Name);
            }
        }
    }
}