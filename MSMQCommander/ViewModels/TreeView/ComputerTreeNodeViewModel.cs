using Caliburn.Micro;

namespace MSMQCommander.ViewModels
{
    public class ComputerTreeNodeViewModel : PropertyChangedBase
    {
        private string _name;

        public ComputerTreeNodeViewModel(string name)
        {
            IsSelected = true;
            IsExpanded = true;

            Name = name;
            Children = new BindableCollection<QueueTypeTreeNodeViewModel>
                           {
                               new QueueTypeTreeNodeViewModel(Name, "Private queues")
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