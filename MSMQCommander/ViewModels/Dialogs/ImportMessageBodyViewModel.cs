using System.Messaging;
using Caliburn.Micro;
using MSMQCommander.Dialogs;
using Microsoft.Win32;
using MsmqLib;

namespace MSMQCommander.ViewModels.Dialogs
{
    public class ImportMessageBodyViewModel : Screen
    {
        private readonly IQueueService _queueService;
        private readonly IDialogService _dialogService;
        private MessageQueue _messageQueue;
        private string _fileToImport;

        public ImportMessageBodyViewModel(IQueueService queueService, IDialogService dialogService)
        {
            _queueService = queueService;
            _dialogService = dialogService;
            UseDeadLetterQueue = true;
        }

        public string Title
        { 
            get { return "Import message body"; } 
        }

        public void Initialize(MessageQueue messageQueue)
        {
            _messageQueue = messageQueue;
        }

        public bool UseDeadLetterQueue { get; set; }

        public string QueueName
        {
            get
            {
                if (_messageQueue == null)
                    return string.Empty;

                return _messageQueue.FormatName;
            }
        }

        public string FileToImport
        {
            get { return _fileToImport; }
            set
            {
                _fileToImport = value;
                NotifyOfPropertyChange(() => FileToImport);
                NotifyOfPropertyChange(() => CanImport);
            }
        }

        public bool CanImport
        {
            get { return !string.IsNullOrWhiteSpace(FileToImport); }
        }

        public void Import()
        {
            string errorMessage;
            if (false == _queueService.ImportMessageBody(_messageQueue, FileToImport, out errorMessage, UseDeadLetterQueue))
            {
                _dialogService.ShowError("Failed to create message: {0}", errorMessage);
            }
            TryClose(true);
        }

        public void SelectFileToImport()
        {
            var openFileDialog = new OpenFileDialog();
            if(true == openFileDialog.ShowDialog())
            {
                FileToImport = openFileDialog.FileName;
            }
        }

    }
}
