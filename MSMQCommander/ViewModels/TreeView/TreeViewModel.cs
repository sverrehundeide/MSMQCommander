using Caliburn.Micro;

namespace MSMQCommander.ViewModels
{
    public class TreeViewModel : Screen
    {
        public BindableCollection<ComputerTreeNodeViewModel> Computers { get; private set; }

        public TreeViewModel(ComputerTreeNodeViewModel computerTreeNodeViewModel)
        {
            Computers = new BindableCollection<ComputerTreeNodeViewModel> { computerTreeNodeViewModel };
        }
    }
}