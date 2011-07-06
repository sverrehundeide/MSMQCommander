using System.Messaging;

namespace MsmqLib
{
    public class QueueService
    {
        public MessageQueue[] GetPrivateQueues(string computerName)
        {
            return MessageQueue.GetPrivateQueuesByMachine(computerName);
        }

        public void Create(string queuePath)
        {
            if (!MessageQueue.Exists(queuePath))
                MessageQueue.Create(queuePath);
        }

        public void DeleteQueue(string queuePath)
        {
            MessageQueue.Delete(queuePath);
        }
    }
}