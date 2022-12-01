namespace OpenSDK
{
    public static class Path
    {
        public static string Join(params string[] paths)
        {
            var result = paths.Aggregate(AppDomain.CurrentDomain.BaseDirectory, (current, path) => current + path + "\\");
            result = result[new Range(0,result.Length-1)];
            return result;
        }
    }
}