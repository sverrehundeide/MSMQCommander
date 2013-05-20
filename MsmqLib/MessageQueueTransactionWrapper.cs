using System;
using System.Collections.Generic;
using System.Linq;
using System.Messaging;
using System.Text;
using System.Transactions;

namespace MsmqLib
{
    public static class MessageQueueExtensions
    {
        public static void Execute(this MessageQueue queue, Action<MessageQueue, MessageQueueTransactionType> action)
        {
            if (queue.Transactional)
            {
                ExecuteTransactional(queue, action);
            }
            else
            {
                ExecuteNonTransactional(queue, action);
            }
        }

        private static void ExecuteTransactional(MessageQueue queue, Action<MessageQueue, MessageQueueTransactionType> action)
        {
            using (var transaction = new TransactionScope())
            {
                action.Invoke(queue, MessageQueueTransactionType.Single);
                transaction.Complete();
            }
        }

        private static void ExecuteNonTransactional(MessageQueue queue, Action<MessageQueue, MessageQueueTransactionType> action)
        {
			action.Invoke(queue, MessageQueueTransactionType.Automatic);
        }
    }
}
