using System;
using System.Diagnostics;

namespace MsmqLib
{
    /// <summary>
    /// Contains basic information about a System.Message, like ID, label and SentTime
    /// </summary>
    [DebuggerDisplay("Id = {Id}, Label = {Label}, SentTime = {SentTime}")]
    public class MessageInfo
    {
        public string Id { get; private set; }
        public string Label { get; private set; }
        public DateTime SentTime { get; private set; }

        public MessageInfo(string id, string label, DateTime sentTime)
        {
            Id = id;
            Label = label;
            SentTime = sentTime;
        }
    }
}