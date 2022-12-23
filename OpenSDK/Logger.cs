using System.Collections;

namespace OpenSDK;
/// <summary>
/// Print message in the console.
/// 向控制台打印信息
/// </summary>
/// <typeparam name="T">Service type. 服务类</typeparam>
public static class Logger<T>
{
    private static void Log(string type, IEnumerable? messages, ConsoleColor color)
    {
        Console.ForegroundColor = color;
        Console.Write("[{0}] [{1}] {2}: ", DateTime.Now, typeof(T), type);
        Console.ResetColor();
        if (messages == null)
        {
            return;
        }
        foreach (var message in messages)
        {
            Console.Write(message + " ");
        }
        Console.Write("\n");
    }
    public static void Error(params string?[]? messages)
    {
        Log("Error", messages, ConsoleColor.DarkRed);
    }

    public static void Info(params string?[]? messages)
    {
        Log("Info", messages, ConsoleColor.Blue);
    }

    public static void Result(params string?[]? messages)
    {
        Log("Success",messages,ConsoleColor.Green);
    }

    public static void Warning(params string?[]? messages)
    {
        Log("Warning",messages,ConsoleColor.Yellow);
    }
}