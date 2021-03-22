using System;

namespace Twitch.DataStrucs
{
    public class LogMessage
    {
        public string message;
        public string source;
        public ConsoleColor color = ConsoleColor.Cyan;
        public Exception exception = null;
    }

    public class CommandExecutionResult
    {
        public bool isSuccessful;
        public Reason reason = Reason.None;
        public Exception exception;
    }

    public enum Reason
    {
        None,
        CommandNotFound,
        ExecutionError
    }
}