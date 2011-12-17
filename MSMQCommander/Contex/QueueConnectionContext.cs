using Caliburn.Micro;
using MSMQCommander.Events;

namespace MSMQCommander.Contex
{
    public class QueueConnectionContext
    {
        private readonly IEventAggregator _eventAggregator;
        public string ComputerName { get; private set; }

        private const string DefaultComputerName = ".";

        public QueueConnectionContext(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            ComputerName = DefaultComputerName;
        }

        public void UpdateComputerName(string newComputerName)
        {
            ComputerName = newComputerName;
            _eventAggregator.Publish(new QueueConnectionChangedEvent(ComputerName));
        }
    }
}