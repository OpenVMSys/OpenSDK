using System.Collections;

namespace OpenSDK;

public static class Logger
{
    private static void Log(string service, string type, IEnumerable messages, ConsoleColor color)
    {
        Console.ForegroundColor = color;
        Console.Write("[{0}] [OpenVMSys3:{1}] {2}: ", DateTime.Now, service, type);
        Console.ResetColor();
        foreach (var message in messages)
        {
            Console.Write(message + " ");
        }
    }
    public static void Error(string service, params string[] messages)
    {
        Log(service, "Error", messages, ConsoleColor.DarkRed);
    }

    public static void Info(string service, params string[] messages)
    {
        Log(service, "Info", messages, ConsoleColor.Blue);
    }

    public static void Result(string service, params string[] messages)
    {
        Log(service,"Success",messages,ConsoleColor.Green);
    }
}