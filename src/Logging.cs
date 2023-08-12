namespace CSRestAPI
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public static class Logging
    {
        public static void Log(VerbosityLevel level, string message, params object[] args)
        {
            Log(level, new Dictionary<string, string>(), message, args);
        }

        public static void LogTrace(string message, params object[] args)
        {
            Log(VerbosityLevel.Trivia, new Dictionary<string, string>(), message, args);
        }

        public static void LogTrace(IDictionary<string, string> values, string message, params object[] args)
        {
            Log(VerbosityLevel.Trivia, values, message, args);
        }

        public static void LogInfo(string message, params object[] args)
        {
            Log(VerbosityLevel.SystemChatter, new Dictionary<string, string>(), message, args);
        }

        public static void LogInfo(IDictionary<string, string> values, string message, params object[] args)
        {
            Log(VerbosityLevel.SystemChatter, values, message, args);
        }

        public static void LogError(string message, params object[] args)
        {
            Log(VerbosityLevel.Significants, new Dictionary<string, string>(), message, args);
        }

        public static void LogError(IDictionary<string, string> values, string message, params object[] args)
        {
            Log(VerbosityLevel.Significants, values, message, args);
        }

        public static void Log(VerbosityLevel level, IDictionary<string, string> values, string message, params object[] args)
        {
            var sb = new StringBuilder();
            sb.AppendFormat("DateTime={0} ", DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"));
            foreach (var key in values.Keys)
            {
                sb.AppendFormat("{0}={1} ", key, values[key]);
            }
            sb.Append("\n");
            sb.AppendFormat(message, args);

            Roost.Birdsong.Tweet(level, 0, sb.ToString());
        }
    }
}
