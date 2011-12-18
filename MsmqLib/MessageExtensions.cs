using System.Messaging;
using System.Text;

namespace MsmqLib
{
    public static class MessageExtensions
    {
        public static string GetMessageBodyAsString(this Message message)
        {
            var encoding = new UTF8Encoding();
            return encoding.GetString(GetBodyAsByteArray(message), 0, (int)message.BodyStream.Length);
        }

        public static byte[] GetBodyAsByteArray(Message msg)
        {
            msg.BodyStream.Position = 0;
            var bodyAsByteArray = new byte[msg.BodyStream.Length];
            msg.BodyStream.Read(bodyAsByteArray, 0, (int)msg.BodyStream.Length);
            return bodyAsByteArray;
        }
    }
}
