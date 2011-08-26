using Caliburn.Micro;

namespace MSMQCommander.ViewModels
{
    public class TreeViewModel : Screen
    {
        public BindableCollection<ComputerTreeNodeViewModel> Computers { get; private set; }

        public TreeViewModel(IEventAggregator eventAggregator)
        {
            Computers = new BindableCollection<ComputerTreeNodeViewModel> {new ComputerTreeNodeViewModel(eventAggregator, ".")};
        }
    }
}