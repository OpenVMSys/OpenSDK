namespace OpenSDK
{
    internal class PluginConfig
    {
        public readonly string PluginName;
        public readonly bool IsEnabled;
        public PluginConfig(string pluginName, bool isEnabled)
        {
            PluginName = pluginName;
            IsEnabled = isEnabled;
        }
    }
}