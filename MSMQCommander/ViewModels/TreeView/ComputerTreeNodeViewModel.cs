using System.Windows;
using Caliburn.Micro;
using MSMQCommander.Contex;

namespace MSMQCommander.ViewModels
{
    public class ComputerTreeNodeViewModel : PropertyChangedBase
    {
        private readonly QueueConnectionContext _queueConnectionContext;

        public ComputerTreeNodeViewModel(QueueConnectionContext queueConnectionContext, QueueTypeTreeNodeViewModel queueTypeTreeNodeViewModel)
        {
            _queueConnectionContext = queueConnectionContext;
            IsSelected = true;
            IsExpanded = true;

            Children = new BindableCollection<QueueTypeTreeNodeViewModel>
                           {
                               queueTypeTreeNodeViewModel,
                           };
        }

        public BindableCollection<QueueTypeTreeNodeViewModel> Children { get; private set; } 

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