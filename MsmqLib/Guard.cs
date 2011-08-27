using System;
using System.Messaging;

namespace MsmqLib
{
    public static class Guard
    {
         public static void QueueExists(string queuePath)
         {
             if (!MessageQueue.Exists(queuePath))
                 throw new InvalidOperationException(string.Format("Qeueue '{0}' does not exist", queuePath));
         }
    }
}