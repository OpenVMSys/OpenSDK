using OpenSDK;

namespace OpenSDK
{
    public class ConfigItem
    {
        public ConfigItem(string name, string value)
        {
            Name = name;
            Value = value;
        }
        public string Name { get; }
        public string Value { get; }
    }
    public class ConfReader<T>
    {
        public static T Read(T targetObj, string filePath)
        {
            Logger<ConfReader<T>>.Info("Reading configuration from file:", filePath);
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
                            Logger<ConfReader<T>>.Error("Configuration file read failed:",ex.Message);
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                Logger<ConfReader<T>>.Error("Configuration file read failed:",ex.Message);
                return targetObj;
            }

            return targetObj;
        }
    }
}

public class ConfReader
{
    public static List<ConfigItem> GetGroup(string groupName, string filePath)
    {
        var result = new List<ConfigItem>();
        var stream = new FileStream(filePath, FileMode.Open);
        var reader = new StreamReader(stream, System.Text.Encoding.UTF8);
        var raw = reader.ReadLine();
        while (raw != null)
        {
            if (raw.Contains("[") && raw.Contains("]"))
            {
                if (raw.Replace("[","").Replace("]","") == groupName)
                {
                    break;
                }
            }
            raw = reader.ReadLine();
        }

        raw = reader.ReadLine();
        while (raw != null && raw == raw.Replace("[","").Replace("]",""))
        {
            try
            {
                var data = raw.Split("=");
                result.Add(new ConfigItem(data[0],data[1]));
            }
            catch
            {
                Logger<ConfReader>.Warning("Broken config line",raw,"in config file",filePath);
            }
            raw = reader.ReadLine();
        }

        return result;
    }

    public static string? GetConf(IEnumerable<ConfigItem> configItems, string key)
    {
        return (from configItem in configItems where configItem.Name == key select configItem.Value).FirstOrDefault();
    }
}
