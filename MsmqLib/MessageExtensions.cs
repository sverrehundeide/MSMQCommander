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

        public static byte[] GetBodyAsByteArray(this Message message)
        {
            message.BodyStream.Position = 0;
            var bodyAsByteArray = new byte[message.BodyStream.Length];
            message.BodyStream.Read(bodyAsByteArray, 0, (int)message.BodyStream.Length);
            return bodyAsByteArray;
        }
    }
}
