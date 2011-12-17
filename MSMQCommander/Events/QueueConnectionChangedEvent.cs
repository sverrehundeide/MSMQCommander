namespace MSMQCommander.Events
{
    public class QueueConnectionChangedEvent
    {
        public string NewMachineName { get; private set; }

        public QueueConnectionChangedEvent(string newMachineName)
        {
            NewMachineName = newMachineName;
        }
    }
}
