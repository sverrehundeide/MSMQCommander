namespace MsmqLib
{
    public class QueuePathHelper
    {
        public static string CreateQueuePathForPrivateQueue(string computerName, string queueName, bool isTransactional)
        {
            return computerName + @"\private$\" + queueName;
        } 
    }
}