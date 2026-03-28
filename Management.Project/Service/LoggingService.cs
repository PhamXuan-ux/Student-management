using System;
using System.IO;

namespace Student.Management.Services
{
    public class LoggingService
    {
        private static LoggingService _instance;

        private static readonly object _lock = new object();

        private readonly string _logFilePath;

        private LoggingService()
        {
            _logFilePath = Path.Combine(Directory.GetCurrentDirectory(), "log.txt");
        }
        public static LoggingService GetInstance()
        {
            lock (_lock)
            {
                if (_instance == null)
                {
                    _instance = new LoggingService();
                }
                return _instance;
            }
        }
        public void Log(string message)
        {
            string logMessage = $"[{DateTime.Now:HH:mm:ss}] {message}";
            File.AppendAllText(_logFilePath, logMessage + Environment.NewLine);
            Console.WriteLine(logMessage);
        }
    }
}
