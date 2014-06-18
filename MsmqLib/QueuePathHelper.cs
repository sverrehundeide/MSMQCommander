namespace MsmqLib
{
    public class QueuePathHelper
    {
        public static string CreateQueuePathForPrivateQueue(string computerName, string queueName, bool isTransactional)
        {
            return string.Format("{0}\\private$\\{1}", computerName, queueName);
        }
		public static string AddComputerNameToQueue(string computerName, string queueName)
		{
			return string.Format("{0}\\{1}", computerName, queueName);
		} 
    }
}