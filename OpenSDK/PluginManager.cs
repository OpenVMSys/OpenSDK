namespace OpenSDK
{
    public class PluginManager
    {
        public OpenPluginCore PluginCore = new();
        private static List<PluginConfig> GetConfig()
        {
            var pluginConfigList = new List<PluginConfig>();
            try
            {
                //使用文件流读取插件配置
                var configStream = new FileStream(System.IO.Path.Join(AppDomain.CurrentDomain.BaseDirectory, "pluginConfig"), FileMode.OpenOrCreate);
                var configReader = new StreamReader(configStream, System.Text.Encoding.UTF8);
                var rawConfig = configReader.ReadToEnd();
                configStream.Flush();
                configReader.Close();
                configStream.Close();
                //在这里要完成后续的插件config格式化读取
                var pluginConfigs = rawConfig.Split("\n");
                foreach (var pluginConfig in pluginConfigs)
                {
                    if (pluginConfig.Split("\t").Length > 1)
                    {
                        pluginConfigList.Add(new PluginConfig(pluginConfig.Split("\t")[0], pluginConfig.Split("\t")[1] == "True"));
                    }
                }
                return pluginConfigList;
            }
            catch (Exception e)
            {
                PluginOutput.PrintError("Plugin load error: " + e.Message);
                try
                {
                    var configStream = new FileStream(Path.Join("pluginConfig"), FileMode.OpenOrCreate);
                    var configWriter = new StreamWriter(configStream, System.Text.Encoding.UTF8);
                    configWriter.Write("");
                    configWriter.Flush();
                    configStream.Flush();
                    configWriter.Close();
                    configStream.Close();
                    return pluginConfigList;
                }
                catch
                {
                    PluginOutput.PrintError("Plugin Initialize Fail, please check your environment. Now exiting...");
                    Environment.Exit(203);
                }
                return pluginConfigList;
            }
        }
        private static bool IsEnabled(List<PluginConfig> configs, string pluginName)
        {
            foreach (var config in configs)
            {
                if (config.PluginName == pluginName && config.IsEnabled)
                {
                    return true;
                }
            }
            return false;
        }
        public static bool IsEnabled(string pluginName)
        {
            return IsEnabled(GetConfig(), pluginName);
        }
        public static void RenewConfig()
        {
            var nowConfigs = GetConfig();
            var newerPlugins = OpenPluginCore.LoadAllPlugins();
            var nowList = new List<string>();
            foreach (var nowConfig in nowConfigs)
            {
                nowList.Add(nowConfig.PluginName);
            }
            foreach (var newerPlugin in newerPlugins)
            {
                if (!nowList.Contains(newerPlugin.GetType().Name))
                {
                    WriteConfig(newerPlugin.GetType().Name, false);
                }
            }
        }
        private static void WriteConfig(string pluginName, bool isEnabled)
        {
            try
            {
                var configFileStream = new FileStream(Path.Join("pluginConfig"), FileMode.Append);
                var configFileWriter = new StreamWriter(configFileStream, System.Text.Encoding.UTF8);
                configFileWriter.Write(pluginName + "\t" + isEnabled + "\n");
                configFileWriter.Flush();
                configFileWriter.Close();
            }
            catch (Exception ex)
            {
                PluginOutput.PrintError("Plugin config set error: " + ex.Message);
            }
        }
        public void TogglePlugin(string pluginName)
        {
            var configList = GetConfig();
            var beforeToggle = IsEnabled(pluginName);
            var plugin = configList.Find((config) => config.PluginName.Equals(pluginName));
            if (plugin == null)
            {
                PluginOutput.PrintError("Plugin: " + pluginName + " does not exist");
                return;
            }
            configList.Remove(plugin);
            configList.Add(new PluginConfig(pluginName, !beforeToggle));
            PluginOutput.PrintResult("Plugin: " + pluginName + (beforeToggle ? " dis" : " en") + "abled");
            File.Delete(Path.Join("pluginConfig"));
            foreach (var config in configList)
            {
                WriteConfig(config.PluginName, config.IsEnabled);
            }
        }
    }
}
