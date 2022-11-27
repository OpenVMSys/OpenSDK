using System;

namespace OpenSDK
{
    public class PluginOutput
    {
        public static void PrintError(params string[] error)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("\nError: \n");
            Console.ResetColor();
            Console.WriteLine(error);
        }
        public static void PrintResult(params string[] result)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("\nSuccess: \n");
            Console.ResetColor();
            Console.WriteLine(result);
        }
    }
}