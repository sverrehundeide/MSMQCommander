using System.Windows;
using Caliburn.Micro;
using MSMQCommander.Contex;
using MSMQCommander.ViewModels.Dialogs;
using MSMQCommander.Views.Dialogs;

namespace MSMQCommander.Dialogs
{
    public interface IDialogService
    {
        MessageBoxResult AskQuestion(string question, string caption, MessageBoxButton button);
        void ConnectToComputer();
        void ShowError(string errorMessageFormat, params object[] args);
    }

    public class DialogService : IDialogService
    {
        private readonly IWindowManager _windowManager;
        private readonly QueueConnectionContext _queueConnectionContext;

        public DialogService(IWindowManager windowManager, QueueConnectionContext queueConnectionContext)
        {
            _windowManager = windowManager;
            _queueConnectionContext = queueConnectionContext;
        }

        public MessageBoxResult AskQuestion(string question, string caption, MessageBoxButton button)
        {
            return MessageBox.Show(question, caption, button, MessageBoxImage.Question);
        }

        public void ConnectToComputer()
        {
            var viewModel = (ConfigureConnectionViewModel)ViewModelLocator.LocateForViewType(typeof (ConfigureConnectionView));
            if (_windowManager.ShowDialog(viewModel) == true)
            {
                _queueConnectionContext.UpdateComputerName(viewModel.ComputerName);
            }
        }

        public void ShowError(string errorMessageFormat, params object[] args)
        {
            var message = string.Format(errorMessageFormat, args);
            MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}