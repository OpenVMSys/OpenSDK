using System;
using System.IO;

namespace OpenSDK
{
    public class ConfReader<T>
    {
        public static T Read(T targetObj, string filePath)
        {
            try
            {
                var t = typeof(T);
                var stream = new FileStream(filePath, FileMode.Open);
                var reader = new StreamReader(stream, System.Text.Encoding.UTF8);
                var lineData = reader.ReadToEnd().Split("\n");
                stream.Close();
                reader.Close();
                foreach (var propertyInfo in t.GetProperties())
                {
                    foreach (var data in lineData)
                    {
                        if (data[0] == '#') { continue; }
                        var splitData = data.Split('=');
                        if (splitData.Length != 2) { continue; }
                        var key = splitData[0];
                        var value = splitData[1];
                        if (propertyInfo.Name != key) { continue; }
                        try
                        {
                            propertyInfo.SetValue(targetObj, value, null);
                        }
                        catch(Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                    }
                }
                return targetObj;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return targetObj;
            }
        }
    }
}
