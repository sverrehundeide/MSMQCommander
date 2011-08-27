using System.Messaging;

namespace MsmqLib.Mapping
{
    public static class MessageMapping
    {
         public static MessageInfo ToMessageInfo(this Message message)
         {
             return new MessageInfo(message.Id, message.Label, message.SentTime);
         }
    }
}