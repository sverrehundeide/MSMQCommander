using System.IO;
using System.Messaging;
using Caliburn.Micro;
using MSMQCommander.Contex;
using MSMQCommander.Events;
using MsmqLib;
using Message = System.Messaging.Message;

namespace MSMQCommander.ViewModels
{
    public class MessageDetailsViewModel : 
        PropertyChangedBase,
        IHandle<MessageSelectedEvent>
    {
        private readonly IQueueService _queueService;
        private readonly MessageQueue _messageQueue;
        private Message _message;

        public MessageDetailsViewModel(CurrentSelectedQueueContext currentSelectedQueueContext, IEventAggregator _eventAggregator, IQueueService queueService)
        {
            _queueService = queueService;
            _messageQueue = currentSelectedQueueContext.CurrentSelectedMessageQueue;
            _eventAggregator.Subscribe(this);
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
            NotifyOfPropertyChange(() => Body);
            NotifyOfPropertyChange(() => DestinationQueue);
            NotifyOfPropertyChange(() => ResponseQueue);
            NotifyOfPropertyChange(() => AdministrationQueue);
            NotifyOfPropertyChange(() => TransactionStatusQueue);
        }

        public string Body
        {
            get
            {
                if (_message == null)
                    return string.Empty;

                var reader = new StreamReader(_message.BodyStream);
                var text = reader.ReadToEnd();
                return text;
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
    }
}