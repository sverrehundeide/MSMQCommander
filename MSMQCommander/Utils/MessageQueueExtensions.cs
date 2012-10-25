using System.Messaging;

namespace MSMQCommander.Utils
{
    public static class MessageQueueExtensions
    {
        public static string GetQueueNameExcludingQueueType(this MessageQueue queue)
        {
            const string backSlash = @"\";
            var indexOfLastSlash = queue.FormatName.LastIndexOf(backSlash, System.StringComparison.Ordinal);
            return queue.FormatName.Substring(indexOfLastSlash + backSlash.Length);
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