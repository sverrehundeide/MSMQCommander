using System;
using System.Collections.Generic;
using System.Messaging;
using MsmqLib.Mapping;

namespace MsmqLib
{
    public interface IQueueService
    {
        MessageQueue[] GetPrivateQueues(string computerName);
        MessageQueue CreateQueue(string queuePath);
        void DeleteQueue(string queuePath);
        void CreateMessage(string queuePath, object body, string label = null);
        IEnumerable<MessageInfo> GetMessageInfos(string queuePath, string labelFilter = null);
        void ClearMessages(string queuePath, string labelFilter = null);
        IEnumerable<MessageInfo> GetMessageInfos(MessageQueue queue, string labelFilter = null);
        Message GetFullMessage(MessageQueue messageQueue, string messageId);
        int GetMessageCount(MessageQueue messageQueue);
        MessageQueue GetJournalQueue(MessageQueue messageQueue);
        void PurgeMessages(MessageQueue messageQueue);
        bool TryConnect(string machineName, out string errorMessage);
    }

    public class QueueService : IQueueService
    {
        public MessageQueue[] GetPrivateQueues(string computerName)
        {
            return MessageQueue.GetPrivateQueuesByMachine(computerName);
        }

        public MessageQueue CreateQueue(string queuePath)
        {
            if (!MessageQueue.Exists(queuePath))
                return MessageQueue.Create(queuePath);

            return new MessageQueue(queuePath);
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
            var queue = new MessageQueue(queuePath);
            return GetMessageInfos(queue, labelFilter);
        }

        public IEnumerable<MessageInfo> GetMessageInfos(MessageQueue queue, string labelFilter = null)
        {
            queue.MessageReadPropertyFilter.ClearAll();
            queue.MessageReadPropertyFilter.Id = true;
            queue.MessageReadPropertyFilter.Label = true;
            queue.MessageReadPropertyFilter.SentTime = true;

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

        public Message GetFullMessage(MessageQueue messageQueue, string messageId)
        {
            messageQueue.MessageReadPropertyFilter.SetAll();
            messageQueue.MessageReadPropertyFilter.SourceMachine = true;
            return messageQueue.PeekById(messageId);
        }

        public int GetMessageCount(MessageQueue messageQueue)
        {
            messageQueue.MessageReadPropertyFilter.ClearAll();
            var enumerator = messageQueue.GetMessageEnumerator2();
            var messageCount = 0;
            while (enumerator.MoveNext())
            {
                messageCount++;
            }
            return messageCount;
        }

        public MessageQueue GetJournalQueue(MessageQueue messageQueue)
        {
            string journalPath = messageQueue.Path + ";JOURNAL";
            return new MessageQueue(journalPath);
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

        public void PurgeMessages(MessageQueue messageQueue)
        {
            messageQueue.Purge();
        }

        public bool TryConnect(string machineName, out string errorMessage)
        {
            try
            {
                MessageQueue.GetPrivateQueuesByMachine(machineName);
            }
            catch (Exception e)
            {
                errorMessage = e.Message;
                return false;
            }
            errorMessage = null;
            return true;
        }
    }
}