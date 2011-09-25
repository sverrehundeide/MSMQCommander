using System;
using System.Reflection;

namespace MSMQCommander.Utils
{
    public class VersionInformation
    {
        public static string GetMajorAndMinorVersion()
        {
            var assemblyVersion = Assembly.GetExecutingAssembly().GetName().Version;
            return string.Format("{0}.{1}", assemblyVersion.Major, assemblyVersion.Minor);
        }

        public static DateTime GetBuildDate()
        {
            // From http://stackoverflow.com/questions/1600962/displaying-the-build-date
            var assemblyVersion = Assembly.GetEntryAssembly().GetName().Version;
            var buildDateTime = new DateTime(2000, 1, 1).Add(new TimeSpan(
                TimeSpan.TicksPerDay * assemblyVersion.Build + // days since 1 January 2000
                TimeSpan.TicksPerSecond * 2 * assemblyVersion.Revision)); // seconds since midnight, (multiply by 2 to get original)
            return buildDateTime;
        }
    }
}