using System;
using Caliburn.Micro;

namespace MSMQCommander.Utils
{
    public class ConsoleLogger : ILog
    {
        public void Info(string format, params object[] args)
        {
            Console.WriteLine(string.Format(format, args));
        }

        public void Warn(string format, params object[] args)
        {
            Console.WriteLine(string.Format(format, args));
        }

        public void Error(Exception exception)
        {
            Console.WriteLine(exception.ToString());
        }
    }
}