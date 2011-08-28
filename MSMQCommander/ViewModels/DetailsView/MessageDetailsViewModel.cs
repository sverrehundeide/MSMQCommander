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
    }
}