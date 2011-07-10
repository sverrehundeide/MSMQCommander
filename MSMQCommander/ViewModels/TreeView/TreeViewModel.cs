using System.ComponentModel.Composition;
using System.Windows;
using Caliburn.Micro;

namespace MSMQCommander.ViewModels
{
    [Export(typeof(TreeViewModel))]
    public class TreeViewModel : Screen
    {
        public BindableCollection<ComputerTreeNodeViewModel> Computers { get; private set; }

        public TreeViewModel()
        {
            Computers = new BindableCollection<ComputerTreeNodeViewModel> {new ComputerTreeNodeViewModel(".")};
        }
    }
}