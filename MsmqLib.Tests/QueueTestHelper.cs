using System.Linq;
using System.Messaging;

namespace MsmqLib.Tests
{
    static internal class QueueTestHelper
    {
        public static MessageQueue CreatePrivateQueue(string computerName, string testQueuePrefix, int queueSequenceNumber, bool isTransactional = false)
        {
            var queueName = testQueuePrefix + queueSequenceNumber;
            return CreatePrivateQueue(computerName, queueName, isTransactional);
        }

        public static MessageQueue CreatePrivateQueue(string computerName, string queueName, bool isTransactional)
        {
            var queuePath = CreateQueuePathForPrivateQueue(computerName, queueName, isTransactional);
            var queueService = new QueueService();
            return queueService.CreateQueue(queuePath, isTransactional);
        }

        public static string CreateQueuePathForPrivateQueue(string computerName, string queueName, bool isTransactional)
        {
            var postfix = isTransactional ? "_transactional" : string.Empty;
            return computerName + @"\private$\" + queueName + postfix;
        }

        public static void CleanupPrivateTestQueues(string computerName, string partialTestQueueName)
        {
            var queueService = new QueueService();
            var privateQueues = queueService.GetPrivateQueues(computerName);

            var testQueues = privateQueues
                .Where(q => q.QueueName.ToLower().Contains(@"private$\" + partialTestQueueName.ToLower()));

            foreach (var queue in testQueues)
            {
                queueService.DeleteQueue(queue.Path);
            }
        }

        public static void DeletePrivateQueueIfExists(string computerName, string queueName, bool isTransactional)
        {
            var queuePath = CreateQueuePathForPrivateQueue(computerName, queueName, isTransactional);
            if (MessageQueue.Exists(queuePath))
                new QueueService().DeleteQueue(queuePath);
        }
    }
}