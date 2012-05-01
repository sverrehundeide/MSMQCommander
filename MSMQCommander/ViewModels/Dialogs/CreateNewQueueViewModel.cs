using System;
using Caliburn.Micro;
using MSMQCommander.Contex;
using MSMQCommander.Dialogs;
using MsmqLib;

namespace MSMQCommander.ViewModels.Dialogs
{
    public class CreateNewQueueViewModel : Screen
    {
        private readonly IQueueService _queueService;
        private readonly QueueConnectionContext _queueConnectionContext;
        private readonly IDialogService _dialogService;

        public CreateNewQueueViewModel(IQueueService queueService, QueueConnectionContext queueConnectionContext, IDialogService dialogService)
        {
            _queueService = queueService;
            _queueConnectionContext = queueConnectionContext;
            _dialogService = dialogService;
        }

        public string Title
        {
            get { return "Create new queue"; }
        }

        private string _queueName;
        public string QueueName
        {
            get { return _queueName; }
            set
            {
                _queueName = value;
                NotifyOfPropertyChange(() => CanCreateQueue);
            }
        }

        public bool IsTransactional { get; set; }

        public bool CanCreateQueue
        {
            get { return !string.IsNullOrWhiteSpace(QueueName); }
        }

        public void CreateQueue()
        {
            string errorMessage;
            var queuePath = QueuePathHelper.CreateQueuePathForPrivateQueue(_queueConnectionContext.ComputerName,
                                                                           QueueName, IsTransactional);
            if (null == _queueService.CreateQueue(queuePath, IsTransactional, out errorMessage))
            {
                _dialogService.ShowError("Failed to creat the queue '{0}': {1}", _queueName, errorMessage);
            }
            TryClose(true);
        }
    }
}