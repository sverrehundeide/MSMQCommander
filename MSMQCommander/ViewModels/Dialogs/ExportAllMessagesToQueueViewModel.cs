using System.Collections.Generic;
using System.Messaging;
using Caliburn.Micro;
using MSMQCommander.Contex;
using MSMQCommander.Dialogs;
using System.Windows;
using MsmqLib;
using Message = System.Messaging.Message;

namespace MSMQCommander.ViewModels.Dialogs
{

	public class ExportAllMessagesToQueueViewModel : Screen {
        private readonly IQueueService _queueService;
        private readonly QueueConnectionContext _queueConnectionContext;
        private readonly IDialogService _dialogService;

        public ExportAllMessagesToQueueViewModel(IQueueService queueService, QueueConnectionContext queueConnectionContext, IDialogService dialogService) {
            this._queueService = queueService;
            this._queueConnectionContext = queueConnectionContext;
            this._dialogService = dialogService;
        }

        public string Title {
            get { return "Export messages to queue"; }
        }

        private string _queueName;

		public string QueueName {
            get { return this._queueName; }
            set {
                this._queueName = value;
                this.NotifyOfPropertyChange(() => this.CanExport);
            }
        }

		public bool RemoveAfterCopy { get; set; }

		public void Initialize(string sourceQueueName, string messageId)
        {
            this.SourceQueueName = sourceQueueName;
            this.MessageId = messageId;
        }

        protected string MessageId { get; set; }

        public bool CanExport {
            get { return !string.IsNullOrWhiteSpace(this.QueueName); }
        }

        private string SourceQueueName { get; set; }

        public void ExportMessages() {
            string errorMessage;

            var destinationQueuePath = QueuePathHelper.CreateQueuePathForPrivateQueue(this._queueConnectionContext.ComputerName,
                this.QueueName, false);

            using (MessageQueue messageQueue = this._queueService.CreateQueue(destinationQueuePath, false, out errorMessage))
            {
                if (messageQueue == null)
                {
                    this._dialogService.ShowError("Failed to create the queue '{0}': {1}", destinationQueuePath, errorMessage);
					return;
                }

	            MessageQueue sourceMessageQueue = null;
	            if(RemoveAfterCopy)
	            {
					sourceMessageQueue = new MessageQueue(this.SourceQueueName);
	            }

                IEnumerable<Message> messages = null;
                if (this.MessageId == null)
                {
					messages = this._queueService.GetMessages(this.SourceQueueName, includeBody: true, includeExtension: true);
                }
                else
                {
					messages = new List<Message>() { this._queueService.GetFullMessage(this.SourceQueueName, this.MessageId) };
                }
                
                foreach (Message message in messages)
                {
	                if (this._queueService.CreateMessageFromByteArray(
		                messageQueue, message.GetBodyAsByteArray(), message.Extension, out errorMessage, message.Label, false))
	                {
		                if (RemoveAfterCopy)
		                {
			                if (this._queueService.DeleteMessage(sourceMessageQueue, message.Id, out errorMessage) == false)
			                {
				                string question = string.Format("Error while deleting message. \r\n {0} : {1} \r\nShould we continue? ", this.SourceQueueName, errorMessage);
				                if(this._dialogService.AskQuestion(question, "Deleting  error", MessageBoxButton.YesNo) == MessageBoxResult.No)
				                {
									break;
				                }
			                }
		                }
	                }
	                else
	                {
		                string question = string.Format("Error while sending message. \r\n {0} : {1} \r\nShould we continue? ", destinationQueuePath, errorMessage);
		                if (this._dialogService.AskQuestion(question, "Sending error", MessageBoxButton.YesNo) == MessageBoxResult.No)
						{
			                break;
		                }
	                }
                }
            }

            this.TryClose(true);
        }
    }
}