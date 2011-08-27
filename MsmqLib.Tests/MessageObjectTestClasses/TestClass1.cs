using System;

namespace MsmqLib.Tests.MessageObjectTestClasses
{
    [Serializable]
    public class TestClass1
    {
        public string StringValue1 { get; set; }
        public int IntValue1 { get; set; }
    }
}