using System.Messaging;

namespace MSMQCommander.Utils
{
    public static class MessageQueueExtensions
    {
        public static string GetQueueNameExcludingQueueType(this MessageQueue queue)
        {
            return queue.QueueName.Replace(@"private$\", "");
        }
        public static string GetQueueNameIncludingQueueType(this MessageQueue queue)
        {
            var path = queue.Path;
            if (path.ToLower().Contains(";journal"))
            {
                const string queueType = @"private$\";
                var indexOfQueueTypeString = path.IndexOf(queueType);
                var name = path.Substring(indexOfQueueTypeString + queueType.Length);
                return name;
            }
            return queue.QueueName.Replace(@"private$\", "");
        }
    }
}