using System.Collections.Generic;
using System.Messaging;
using Caliburn.Micro;
using MSMQCommander.Contex;
using MSMQCommander.Dialogs;

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
            var sourceQueuePath = QueuePathHelper.AddComputerNameToQueue(this._queueConnectionContext.ComputerName,
                this.SourceQueueName);

            var destinationQueuePath = QueuePathHelper.CreateQueuePathForPrivateQueue(this._queueConnectionContext.ComputerName,
                this.QueueName, false);

            using (MessageQueue messageQueue = this._queueService.CreateQueue(destinationQueuePath, false, out errorMessage))
            {
                if (messageQueue == null)
                {
                    this._dialogService.ShowError("Failed to create the queue '{0}': {1}", destinationQueuePath, errorMessage);
                }

                IEnumerable<Message> messages = null;
                if (this.MessageId == null)
                {
                    messages = this._queueService.GetMessages(sourceQueuePath, includeBody: true, includeExtension: true);
                }
                else
                {
                    messages = new List<Message>() { this._queueService.GetFullMessage(sourceQueuePath, this.MessageId) };
                }
                
                foreach (Message message in messages)
                {
                    if (this._queueService.CreateMessageFromByteArray(
                        messageQueue, message.GetBodyAsByteArray(), message.Extension, out errorMessage, message.Label, false) == false)
                    {
                        this._dialogService.ShowError("Failed to send the message '{0}': {1}", destinationQueuePath, errorMessage);
                    }
                }
            }

            this.TryClose(true);
        }
    }
}