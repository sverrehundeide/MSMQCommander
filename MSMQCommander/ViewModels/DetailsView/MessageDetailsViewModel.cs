using System.IO;
using System.Messaging;
using Caliburn.Micro;
using MSMQCommander.Contex;
using MSMQCommander.Events;
using MsmqLib;
using Message = System.Messaging.Message;
using MSMQCommander.Utils;

namespace MSMQCommander.ViewModels
{
    public class MessageDetailsViewModel : 
        PropertyChangedBase,
        IHandle<MessageSelectedEvent>
    {
        private readonly IQueueService _queueService;
        private readonly MessageQueue _messageQueue;
        private Message _message;

        public MessageDetailsViewModel(CurrentSelectedQueueContext currentSelectedQueueContext, IEventAggregator eventAggregator, IQueueService queueService)
        {
            _queueService = queueService;
            _messageQueue = currentSelectedQueueContext.CurrentSelectedMessageQueue;
            eventAggregator.Subscribe(this);
        }

        public void Handle(MessageSelectedEvent messageSelectedEvent)
        {
            if (messageSelectedEvent.MessageQueue.Path != _messageQueue.Path)
                return;

            var fullMessage = _queueService.GetFullMessage(_messageQueue, messageSelectedEvent.MessageId);
            SetCurrentMessage(fullMessage);
        }

        private void SetCurrentMessage(Message fullMessage)
        {
            _message = fullMessage;
            Refresh();
        }

        public string BodySize
        {
            get
            {
                if (_message == null)
                    return string.Empty;

                return string.Format("{0} bytes", _message.BodyStream.Length);
            }
        }

        public string Body
        {
            get
            {
                if (_message == null)
                    return string.Empty;

                return _message.GetMessageBodyAsString();
            }
        }

        public string DestinationQueue
        {
            get
            {
                if (_message == null || _message.DestinationQueue == null)
                    return string.Empty;

                return _message.DestinationQueue.Path;
            }
        }

        public string ResponseQueue
        {
            get
            {
                if (_message == null || _message.ResponseQueue == null)
                    return string.Empty;

                return _message.ResponseQueue.Path;
            }
        }

        public string AdministrationQueue
        {
            get
            {
                if (_message == null || _message.AdministrationQueue == null)
                    return string.Empty;

                return _message.AdministrationQueue.Path;
            }
        }

        public string TransactionStatusQueue
        {
            get
            {
                if (_message == null || _message.TransactionStatusQueue == null)
                    return string.Empty;

                return _message.TransactionStatusQueue.Path;
            }
        }

        public string ArrivedTime
        {
            get
            {
                if (_message == null)
                    return string.Empty;

                return _message.ArrivedTime.ToString("yyyy-MM-dd HH:mm:ss.fff (zzz)");
            }
        }

        public string SentTime
        {
            get
            {
                if (_message == null)
                    return string.Empty;

                return _message.SentTime.ToString("yyyy-MM-dd HH:mm:ss.fff (zzz)");
            }
        }

        public string TimeToBeReceived
        {
            get
            {
                if (_message == null)
                    return string.Empty;

                return _message.TimeToBeReceived.ToString("G");
            }
        }

        public string TimeToReachQueue
        {
            get
            {
                if (_message == null)
                    return string.Empty;

                return _message.TimeToReachQueue.ToString("G");
            }
        }

        public string Priority
        {
            get
            {
                if (_message == null)
                    return string.Empty;

                return _message.Priority.ToString();
            }
        }

        public string UseDeadLetterQueue
        {
            get
            {
                if (_message == null)
                    return string.Empty;

                return _message.UseDeadLetterQueue.ToYesNo();
            }
        }

        public string UseJournalQueue
        {
            get
            {
                if (_message == null)
                    return string.Empty;

                return _message.UseJournalQueue.ToYesNo();
            }
        }

        public string Recoverable
        {
            get
            {
                if (_message == null)
                    return string.Empty;

                return _message.Recoverable.ToYesNo();
            }
        }

        public string UseAuthentication
        {
            get
            {
                if (_message == null)
                    return string.Empty;

                return _message.UseAuthentication.ToYesNo();
            }
        }

        public string UseEncryption
        {
            get
            {
                if (_message == null)
                    return string.Empty;

                return _message.UseEncryption.ToYesNo();
            }
        }

        public string UseTracing
        {
            get
            {
                if (_message == null)
                    return string.Empty;

                return _message.UseTracing.ToYesNo();
            }
        }

        public string MessageType
        {
            get
            {
                if (_message == null)
                    return string.Empty;

                return _message.MessageType.ToString();
            }
        }
    }
}