namespace SIS.MvcFramework.Logging
{
    using System;
    using System.Threading;

    public class ConsoleLogger : ILogger
    {
        public void Log(string message)
        {
            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [{Thread.CurrentThread.ManagedThreadId}] {message}");
        }
    }
}
