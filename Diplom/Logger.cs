using System;
using System.IO;

namespace ScoreConverter
{
    internal static class Logger
    {
        private static readonly string logFile = "log.txt";
        internal static void Write(string message)
        {
            File.AppendAllText(logFile, $"{DateTime.UtcNow} {message}");
        }
    }
}
