using System.Messaging;
using Caliburn.Micro;
using MSMQCommander.Contex;
using MSMQCommander.Events;

namespace MSMQCommander.ViewModels
{
    public class MessageDetailsViewModel : 
        PropertyChangedBase,
        IHandle<MessageSelectedEvent>
    {
        private readonly MessageQueue _messageQueue;

        public MessageDetailsViewModel(CurrentSelectedQueueContext currentSelectedQueueContext, IEventAggregator _eventAggregator)
        {
            _messageQueue = currentSelectedQueueContext.CurrentSelectedMessageQueue;
            _eventAggregator.Subscribe(this);
        }

        public void Handle(MessageSelectedEvent messageSelectedEvent)
        {
            if (messageSelectedEvent.MessageQueue.Path != _messageQueue.Path)
                return;
        }
    }
}