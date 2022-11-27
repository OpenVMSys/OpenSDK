using System;

namespace OpenSDK
{
    public class Path
    {
        public static string Join(params string[] paths)
        {
            var result = AppDomain.CurrentDomain.BaseDirectory;
            foreach (var path in paths)
            {
                result += path + "\\";
            }
            result = result[new Range(0,result.Length-1)];
            return result;
        }
    }
}