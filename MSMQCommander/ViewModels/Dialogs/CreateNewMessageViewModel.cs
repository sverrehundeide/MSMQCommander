using System.Messaging;
using System.Windows.Documents;
using Caliburn.Micro;
using MSMQCommander.Dialogs;
using MSMQCommander.Views.Dialogs;
using MsmqLib;

namespace MSMQCommander.ViewModels.Dialogs
{
    public class CreateNewMessageViewModel : Screen
    {
        private readonly IQueueService _queueService;
        private readonly IDialogService _dialogService;
        private MessageQueue _messageQueue;
        private CreateNewMessageView _view;

        public CreateNewMessageViewModel(IQueueService queueService, IDialogService dialogService)
        {
            _queueService = queueService;
            _dialogService = dialogService;
        }

        public void Initialize(MessageQueue messageQueue)
        {
            _messageQueue = messageQueue;
        }

        public string Title
        {
            get { return "Create new message"; }
        }

        public string QueueName
        {
            get
            {
                if (_messageQueue == null)
                    return string.Empty;

                return _messageQueue.FormatName;
            }
        }

        protected override void OnViewAttached(object view, object context)
        {
            base.OnViewAttached(view, context);
            _view = (CreateNewMessageView) view;
        }

        public void Save()
        {
            var messageText = GetMessageText();
            string errorMessage;
            if (false == _queueService.CreateMessage(_messageQueue, messageText, out errorMessage))
            {
                _dialogService.ShowError("Failed to create message: {0}", errorMessage);
            }
            TryClose(true);
        }

        private string GetMessageText()
        {
            var textSelection = new TextRange(_view.MessageTextBox.Document.ContentStart,
                                              _view.MessageTextBox.Document.ContentEnd);
            var messageText = textSelection.Text;
            return messageText;
        }
    }
}
