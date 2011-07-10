using System.Messaging;

namespace MSMQCommander.Utils
{
    public static class MessageQueueExtensions
    {
         public static string QueueNameExcludingQueueType(this MessageQueue queue)
         {
             return queue.QueueName.Replace(@"private$\", "");
         }
    }
}