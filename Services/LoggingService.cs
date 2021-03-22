using System;
using System.Collections.Concurrent;
using System.Threading;
public class LoggingService
{
    private static BlockingCollection<Log> logQueue = new BlockingCollection<Log>();
    public static void Initialize()
    {
        Thread thread = new Thread(() =>
        {
            while (true)
            {
                var log = logQueue.Take();

                Console.Write($"{DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss")} ");

                Console.ForegroundColor = log.color;
                Console.Write($"[{log.source}] ");
                Console.ResetColor();

                Console.Write(log.message + Environment.NewLine);
            }
        });
        thread.Start();
    }

    /// <summary>
    /// Logs messages to the Console.
    /// </summary>
    /// <param name="message">The message to log</param>
    /// <param name="source">The message source</param>
    public static void Log(string message, string source)
        => logQueue.Add(new Log() { message = message, source = source });

    /// <summary>
    /// Logs message and exception to the Console
    /// </summary>
    /// <param name="message">The message to log</param>
    /// <param name="source">The message source</param>
    /// <param name="color">The message color</param>
    /// <param name="exception">Exception</param>
    public static void Log(string message, string source, ConsoleColor color, Exception exception)
        => logQueue.Add(new Log() { message = message, source = source, color = color, exception = exception });
}