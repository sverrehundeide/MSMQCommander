using System.IO;
using System.Messaging;
using System.Text;

namespace MsmqLib
{
    public static class MessageExtensions
    {
        public static string GetMessageBodyAsString(this Message message)
        {
            var encoding = new UTF8Encoding();
            return encoding.GetString(GetStreamAsByteArray(message.BodyStream), 0, (int)message.BodyStream.Length);
        }

        public static byte[] GetStreamAsByteArray(Stream stream)
        {
            stream.Position = 0;
            var streamAsByteArray = new byte[stream.Length];
            stream.Read(streamAsByteArray, 0, (int)stream.Length);
            return streamAsByteArray;
        }

        public static string GetExtensionDataAsString(this Message message)
        {
            var encoding = new UTF8Encoding();
            return encoding.GetString(message.Extension, 0, message.Extension.Length);
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
