using System.Messaging;
using System.Windows;
using Caliburn.Micro;
using MSMQCommander.Contex;
using MSMQCommander.ViewModels.Dialogs;
using MSMQCommander.Views.Dialogs;
using Microsoft.Win32;
using MsmqLib;

namespace MSMQCommander.Dialogs
{
    public interface IDialogService
    {
        MessageBoxResult AskQuestion(string question, string caption, MessageBoxButton button);
        void ConnectToComputer();
        void ShowError(string errorMessageFormat, params object[] args);
        void ExportMessageBody(MessageQueue messageQueue, string messageId);
    }

    public class DialogService : IDialogService
    {
        private readonly IWindowManager _windowManager;
        private readonly QueueConnectionContext _queueConnectionContext;
        private readonly IQueueService _queueService;

        public DialogService(IWindowManager windowManager, QueueConnectionContext queueConnectionContext, IQueueService queueService)
        {
            _windowManager = windowManager;
            _queueConnectionContext = queueConnectionContext;
            _queueService = queueService;
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

        public void ExportMessageBody(MessageQueue messageQueue, string messageId)
        {
            var saveFileDialog = new SaveFileDialog();
            if (saveFileDialog.ShowDialog() == true)
            {
                var filePath = saveFileDialog.FileName;
                string errorMessage;
                if (false == _queueService.ExportMessageBody(messageQueue, messageId, filePath, out errorMessage))
                {
                    ShowError("Failed to export to file '{0}': {1}", filePath, errorMessage);
                }
            }
        }
    }
}