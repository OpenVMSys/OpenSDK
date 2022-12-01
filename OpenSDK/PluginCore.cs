using System.Collections;
using System.Reflection;

namespace OpenSDK
{
    public class OpenPluginCore
    {
        public static ArrayList LoadAllPlugins()
        {
            ArrayList allPlugins = new();
            try
            {
                var plugins = Directory.GetFiles(Path.Join("plugins"));

                foreach (var fileName in plugins)
                {
                    if (!fileName.ToUpper().EndsWith(".DLL"))
                    {
                        continue;
                    }
                    try
                    {
                        var loadPlugin = Assembly.LoadFrom(fileName);
                        var types = loadPlugin.GetTypes();
                        foreach (var type in types)
                        {
                            if (type.GetInterface("IOpenVMSysPlugin") != null && type.FullName != null)
                            {
                                allPlugins.Add(loadPlugin.CreateInstance(type.FullName));
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Error("OpenSDK", "Plugin load error", ex.Message);
                    }
                }
            }
            catch
            {
                Logger.Info("OpenSDK","There's no plugin in your plugin directory");
            }
            return allPlugins;
        }

        public static object? PluginMessageTransport(string on, string plugin, params object[] data)
        {
            try
            {
                var pluginList = LoadAllPlugins();
                
                foreach (var pluginObj in pluginList)
                {
                    if (plugin!=pluginObj.GetType().Name || !PluginManager.IsEnabled(plugin))
                    {
                        continue;
                    }
                    var methodInfo = pluginObj.GetType().GetMethod("On" + on);
                    var returnValue = methodInfo==null?null:methodInfo.Invoke(pluginObj, data);
                    return returnValue;
                }
            }
            catch (Exception e)
            {
                Logger.Error("OpenSDK","ERROR IN CALLING PLUGIN:", plugin, "ON CHANNEL", on, "\n", e.Message);
                return null;
            }

            return null;
        }
    }
}