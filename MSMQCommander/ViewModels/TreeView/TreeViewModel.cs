using System.ComponentModel.Composition;
using System.Windows;
using Caliburn.Micro;

namespace MSMQCommander.ViewModels
{
    [Export]
    public class TreeViewModel : Screen
    {
        public BindableCollection<ComputerTreeNodeViewModel> Computers { get; private set; }

        [ImportingConstructor]
        public TreeViewModel(IEventAggregator eventAggregator)
        {
            Computers = new BindableCollection<ComputerTreeNodeViewModel> {new ComputerTreeNodeViewModel(eventAggregator, ".")};
        }
    }
}