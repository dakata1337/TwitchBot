using System;
public class Log
{
    public string message;
    public string source;
    public ConsoleColor color = ConsoleColor.Cyan;
    public Exception exception = null;
}
public class Result
{
    public bool isSuccessful;
    public Exception exception;
}