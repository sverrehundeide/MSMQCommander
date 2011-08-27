using System;
using System.Collections.Generic;
using System.Messaging;
using MsmqLib.Mapping;

namespace MsmqLib
{
    public class QueueService
    {
        public MessageQueue[] GetPrivateQueues(string computerName)
        {
            return MessageQueue.GetPrivateQueuesByMachine(computerName);
        }

        public void CreateQueue(string queuePath)
        {
            if (!MessageQueue.Exists(queuePath))
                MessageQueue.Create(queuePath);
        }

        public void DeleteQueue(string queuePath)
        {
            MessageQueue.Delete(queuePath);
        }

        public void CreateMessage(string queuePath, object body, string label = null)
        {
            Guard.QueueExists(queuePath);

            var message = new Message(body);
            var messageQueue = new MessageQueue(queuePath);
            messageQueue.Send(message, (label ?? string.Empty));
            messageQueue.Close();
        }

        public IEnumerable<MessageInfo> GetMessageInfos(string queuePath, string labelFilter = null)
        {
            Guard.QueueExists(queuePath);

            var queue = new MessageQueue(queuePath)
                            {
                                MessageReadPropertyFilter = {Label = true, SentTime = true}
                            };

            var result = new List<MessageInfo>();

            var messageEnumerator = queue.GetMessageEnumerator2();
            try
            {
                while (messageEnumerator.MoveNext())
                {
                    Message currentMessage = null;
                    try
                    {
                        currentMessage = messageEnumerator.Current;
                    }
                    catch (MessageQueueException e)
                    {
                        //TODO: Use ILog
                        Console.WriteLine(e);
                    }
                    if (currentMessage == null)
                        continue;

                    if (labelFilter == null || currentMessage.Label == labelFilter)
                        result.Add(currentMessage.ToMessageInfo());
                }
            }
            finally
            {
                messageEnumerator.Close();
            }
            queue.Close();
            return result;
        }

        public void ClearMessages(string queuePath, string labelFilter = null)
        {
            Guard.QueueExists(queuePath);

            var queue = new MessageQueue(queuePath)
            {
                MessageReadPropertyFilter = { Label = true }
            };

            var messageEnumerator = queue.GetMessageEnumerator2();
            try
            {
                while (messageEnumerator.MoveNext())
                {
                    Message currentMessage = null;
                    try
                    {
                        currentMessage = messageEnumerator.Current;
                    }
                    catch (MessageQueueException e)
                    {
                        //TODO: Use ILog
                        Console.WriteLine(e);
                    }
                    if (currentMessage == null)
                        continue;

                    if (labelFilter != null && currentMessage.Label != labelFilter)
                        continue;

                    queue.ReceiveById(currentMessage.Id);
                }
            }
            finally
            {
                messageEnumerator.Close();
            }
            queue.Close();
        }
    }
}