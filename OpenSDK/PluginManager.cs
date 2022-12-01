namespace OpenSDK
{
    public static class PluginManager
    {
        public static OpenPluginCore PluginCore = new();
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
                pluginConfigList.AddRange(from pluginConfig in pluginConfigs where pluginConfig.Split("\t").Length > 1 select new PluginConfig(pluginConfig.Split("\t")[0], pluginConfig.Split("\t")[1] == "True"));
                return pluginConfigList;
            }
            catch (Exception e)
            {
                Logger.Error("OpenSDK","Plugin load error:", e.Message);
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
                    Logger.Error("OpenSDK","Plugin Initialize Fail, please check your environment. Now exiting...");
                    Environment.Exit(203);
                }
                return pluginConfigList;
            }
        }
        private static bool IsEnabled(IEnumerable<PluginConfig> configs, string pluginName)
        {
            return configs.Any(config => config.PluginName == pluginName && config.IsEnabled);
        }
        public static bool IsEnabled(string pluginName)
        {
            return IsEnabled(GetConfig(), pluginName);
        }
        public static void RenewConfig()
        {
            var nowConfigs = GetConfig();
            var newerPlugins = OpenPluginCore.LoadAllPlugins();
            var nowList = nowConfigs.Select(nowConfig => nowConfig.PluginName).ToList();
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
                Logger.Error("OpenSDK","Plugin config set error:", ex.Message);
            }
        }
        public static void TogglePlugin(string pluginName)
        {
            var configList = GetConfig();
            var beforeToggle = IsEnabled(pluginName);
            var plugin = configList.Find((config) => config.PluginName.Equals(pluginName));
            if (plugin == null)
            {
                Logger.Error("OpenSDK", "Plugin:", pluginName, "does not exist");
                return;
            }
            configList.Remove(plugin);
            configList.Add(new PluginConfig(pluginName, !beforeToggle));
            Logger.Result("OpenSDK","Plugin:",pluginName,"is now",beforeToggle ? " dis" : " en","abled");
            File.Delete(Path.Join("pluginConfig"));
            foreach (var config in configList)
            {
                WriteConfig(config.PluginName, config.IsEnabled);
            }
        }
    }
}
