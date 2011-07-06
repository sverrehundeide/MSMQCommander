using System.Linq;

namespace MsmqLib.Tests
{
    static internal class QueueTestHelper
    {
        public static void CreatePrivateQueue(string computerName, string testQueuePrefix, int queueSequenceNumber)
        {
            var queueName = testQueuePrefix + queueSequenceNumber;
            CreatePrivateQueue(computerName, queueName);
        }

        public static void CreatePrivateQueue(string computerName, string queueName)
        {
            var queuePath = computerName + @"\private$\" + queueName;
            var queueService = new QueueService();
            queueService.Create(queuePath);
        }


        public static void CleanupPrivateTestQueues(string computerName, string partialTestQueueName)
        {
            var queueService = new QueueService();
            var privateQueues = queueService.GetPrivateQueues(computerName);

            var testQueues = privateQueues
                .Where(q => q.QueueName.ToLower().Contains(@"private$\" + partialTestQueueName));

            foreach (var queue in testQueues)
            {
                queueService.DeleteQueue(queue.Path);
            }
        }
    }
}