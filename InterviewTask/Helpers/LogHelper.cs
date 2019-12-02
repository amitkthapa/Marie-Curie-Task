using System;
using System.IO;

namespace InterviewTask.Helpers
{
    public class LogHelper
    {
        private const string LogFileDirectory = "\\MarieCurie\\Logs\\";


        private string GetLogFileDir()
        {
            var logFileName = DateTime.Now.ToString("yyyyMMdd") + ".txt";
            return $"{LogFileDirectory}/{logFileName}";
        }
        public void LogError(string message)
        {
            var logFileDir = GetLogFileDir();

            using (StreamWriter w = File.AppendText(logFileDir))
            {
                Log(message, w);
            }
        }
        private void Log(string message, TextWriter w)
        {
            w.Write("\n");
            w.WriteLine(message);
        }
    }
}