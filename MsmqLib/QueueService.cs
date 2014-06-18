using System;
using System.Collections.Generic;
using System.IO;
using System.Messaging;
using System.Text;
using System.Xml;
using MsmqLib.Mapping;

namespace MsmqLib
{
    using System.Linq;

    public interface IQueueService
    {
        MessageQueue[] GetPrivateQueues(string computerName);
        MessageQueue CreateQueue(string queuePath, bool isTransactional = false);
        MessageQueue CreateQueue(string queuePath, bool isTransactional, out string errorMessage);
        void DeleteQueue(string queuePath);
        bool DeleteQueue(MessageQueue messageQueue, out string errorMessage);
        void CreateMessage(string queuePath, object body, string label = null);
        bool CreateMessage(MessageQueue messageQueue, object body, out string errorMessage, string label = null, bool useDeadLetterQueue = true);
        void ClearMessages(string queuePath, string labelFilter = null);
        IEnumerable<MessageInfo> GetMessageInfos(string queuePath, string labelFilter = null);
        IEnumerable<MessageInfo> GetMessageInfos(MessageQueue queue, string labelFilter = null);
		IEnumerable<Message> GetMessages(string queuePath, string labelFilter = null, bool includeBody = false, bool includeExtension = false);
		IEnumerable<Message> GetMessages(MessageQueue queue, string labelFilter = null, bool includeBody = false, bool includeExtension = false);
        Message GetFullMessage(MessageQueue messageQueue, string messageId);
		Message GetFullMessage(string queuePath, string messageId);
        int GetMessageCount(MessageQueue messageQueue);
        MessageQueue GetJournalQueue(MessageQueue messageQueue);
        void PurgeMessages(MessageQueue messageQueue);
        bool TryConnect(string machineName, out string errorMessage);
        bool ExportMessageBody(MessageQueue messageQueue, string messageId, string fileName, out string errorMessage);
        bool ImportMessageBody(MessageQueue messageQueue, string fileName, out string errorMessage, bool useDeadletterQueue = true);
        bool DeleteMessage(MessageQueue messageQueue, string messageId, out string errorMessage);
        bool HasAccess(MessageQueue messageQueue);
        bool CreateMessageFromByteArray(MessageQueue messageQueue, byte[] body, byte[] header, out string errorMessage, string label = null, bool useDeadLetterQueue = true);
    }

    public class QueueService : IQueueService
    {
        public MessageQueue[] GetPrivateQueues(string computerName)
        {
            return MessageQueue.GetPrivateQueuesByMachine(computerName);
        }

        public MessageQueue CreateQueue(string queuePath, bool isTransactional)
        {
            if (!MessageQueue.Exists(queuePath))
                return MessageQueue.Create(queuePath, isTransactional);

            return new MessageQueue(queuePath);
        }

        public MessageQueue CreateQueue(string queuePath, bool isTransactional, out string errorMessage)
        {
            errorMessage = null;
            try
            {
                return CreateQueue(queuePath, isTransactional);
            }
            catch(Exception e)
            {
                errorMessage = e.Message;
                return null;
            }
        }

        public void DeleteQueue(string queuePath)
        {
            MessageQueue.Delete(queuePath);
        }

        public bool DeleteQueue(MessageQueue messageQueue, out string errorMessage)
        {
            errorMessage = null;
            try
            {
                DeleteQueue(messageQueue.Path);
                return true;
            }
            catch (Exception e)
            {
                errorMessage = e.Message;
                return false;
            }
        }


        public void CreateMessage(string queuePath, object body, string label = null)
        {
            Guard.QueueExists(queuePath);

            var messageQueue = new MessageQueue(queuePath);
            
            string errorMessage;
            CreateMessage(messageQueue, body, out errorMessage, label);
            messageQueue.Close();
        }

        public bool CreateMessage(MessageQueue messageQueue, object bodyObject, out string errorMessage, string label = null, bool useDeadLetterQueue = true)
        {
            try
            {
                var body = GetXmlBodyObject(bodyObject);
                var message = new Message(body, new XmlMessageFormatter())
                                  {
                                      UseDeadLetterQueue = useDeadLetterQueue
                                  };
                messageQueue.Execute((queue, transactionType) => queue.Send(message, (label ?? string.Empty), transactionType));
            }
            catch (Exception e)
            {
                errorMessage = e.Message;
                return false;
            }
            errorMessage = null;
            return true;
        }

        private static object GetXmlBodyObject(object bodyString)
        {
            object body;
            XmlDocument bodyXmlDocument;
            if (bodyString is String &&
                TryLoadXmlDocument((String)bodyString, out bodyXmlDocument))
            {
                body = bodyXmlDocument;
            }
            else
            {
                body = bodyString;
            }
            return body;
        }

        private static bool TryLoadXmlDocument(string bodyString, out XmlDocument bodyXmlDocument)
        {
            try
            {
                bodyXmlDocument = new XmlDocument();
                bodyXmlDocument.LoadXml(bodyString);
                return true;
            }
            catch (Exception)
            {
                bodyXmlDocument = null;
                return false;
            }
        }

        public bool CreateMessageFromByteArray(MessageQueue messageQueue, byte[] body, byte [] header, out string errorMessage, string label = null, bool useDeadLetterQueue = true) {
            try {
                var message = new Message();
                message.Formatter = new BinaryMessageFormatter();
                message.BodyStream.Write(body, 0, body.Length);
                message.Extension = header;
                messageQueue.Execute((queue, transactionType) => queue.Send(message, (label ?? string.Empty), transactionType));
            }
            catch (Exception e) {
                errorMessage = e.Message;
                return false;
            }
            errorMessage = null;
            return true;
        }

        public IEnumerable<Message> GetMessages(string queuePath, string labelFilter = null, bool includeBody = false, bool includeExtension = false) {
            Guard.QueueExists(queuePath);
            var queue = new MessageQueue(queuePath);
            return this.GetMessages(queue, labelFilter, includeBody, includeExtension);
        }

        public IEnumerable<Message> GetMessages(MessageQueue queue, string labelFilter = null, bool includeBody = false, bool includeExtension = false) {
            queue.MessageReadPropertyFilter.ClearAll();
            queue.MessageReadPropertyFilter.Id = true;
            queue.MessageReadPropertyFilter.Label = true;
            queue.MessageReadPropertyFilter.SentTime = true;
            queue.MessageReadPropertyFilter.Body = includeBody;
            queue.MessageReadPropertyFilter.Extension = includeExtension;

            var result = new List<Message>();
            if (!HasAccess(queue)) {
                return result;
            }

            var messageEnumerator = queue.GetMessageEnumerator2();
            try {
                while (messageEnumerator.MoveNext()) {
                    Message currentMessage = null;
                    try {
                        currentMessage = messageEnumerator.Current;
                    }
                    catch (MessageQueueException e) {
                        //TODO: Use ILog
                        Console.WriteLine(e);
                    }
                    if (currentMessage == null)
                        continue;

                    if (labelFilter == null || currentMessage.Label == labelFilter)
                        result.Add(currentMessage);
                }
            }
            finally {
                messageEnumerator.Close();
            }
            queue.Close();
            return result;
        }

        public IEnumerable<MessageInfo> GetMessageInfos(string queuePath, string labelFilter = null)
        {
            Guard.QueueExists(queuePath);
            var queue = new MessageQueue(queuePath);
            return this.GetMessages(queue, labelFilter).Select(m=> m.ToMessageInfo());
        }

        public IEnumerable<MessageInfo> GetMessageInfos(MessageQueue queue, string labelFilter = null)
        {
            return this.GetMessages(queue, labelFilter).Select(m => m.ToMessageInfo());
        }


        public Message GetFullMessage(string queuePath, string messageId) {
            Guard.QueueExists(queuePath);
            var queue = new MessageQueue(queuePath);
            return this.GetFullMessage(queue, messageId);
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
            if (!HasAccess(messageQueue))
            {
                return messageCount;
            }
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

        public bool ExportMessageBody(MessageQueue messageQueue, string messageId, string fileName, out string errorMessage)
        {
            try
            {
                var message = GetFullMessage(messageQueue, messageId);
                var messageBodyString = message.GetMessageBodyAsString();
                var reader = new StreamReader(message.BodyStream);
                using (var fileStream = new FileStream(fileName, FileMode.Create))
                {
                    byte[] messageAsByteArray = new UTF8Encoding(true).GetBytes(messageBodyString);
                    fileStream.Write(messageAsByteArray, 0, messageAsByteArray.Length);
                }
                reader.Close();
            }
            catch (Exception e)
            {
                errorMessage = e.Message;
                return false;
            }
            errorMessage = null;
            return true;
        }

        public bool ImportMessageBody(MessageQueue messageQueue, string fileName, out string errorMessage, bool useDeadletterQueue = true)
        {
            try
            {
                var fileStream = File.OpenText(fileName);
                var contentsString = fileStream.ReadToEnd();
                return CreateMessage(messageQueue, contentsString, out errorMessage, null, useDeadletterQueue);
            }
            catch (Exception e)
            {
                errorMessage = e.Message;
                return false;
            }
        }

        public bool DeleteMessage(MessageQueue messageQueue, string messageId, out string errorMessage)
        {
            try
            {
                messageQueue.Execute((queue, transactionType) => queue.ReceiveById(messageId, transactionType));
            }
            catch (Exception e)
            {
                errorMessage = e.Message;
                return false;
            }
            errorMessage = null;
            return true;
        }

        public bool HasAccess(MessageQueue messageQueue)
        {
            return messageQueue.CanRead;
        }
    }
}