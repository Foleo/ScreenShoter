using PluginInterface;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ScreenShoter.Assets.Temp
{
    public class PluginResolver
    {
        private static List<IPlugin> plugins = new List<IPlugin>();

        public static List<IPlugin> GetPlugins()
        {
            return plugins;
        }

        public static IPlugin GetPlugin(string hash)
        {
            foreach (var plugin in plugins)
            {
                if (plugin.GetHashCode().ToString() == hash)
                    return plugin;
            }

            return null;
        }

        public static void AddPlugin(IPlugin plugin)
        {
            plugins.Add(plugin);
        }

        public static void AddPlugins(List<IPlugin> plugins)
        {
            plugins.AddRange(plugins);
        }

        public static int LoadPluginsFromDirectory(string path)
        {
            plugins.Clear();

            string[] dllFileNames = null;
            if (Directory.Exists(path))
            {
                dllFileNames = Directory.GetFiles(path, "*.dll");
                Trace.WriteLine(DateTime.Now + " - Loading plug-in from " + Path.GetFullPath(path));
                Trace.Indent();
            }
            else
            {
                Trace.WriteLine("No plug-ins loaded.");
                return 0;
            }

            AppDomain.CurrentDomain.ReflectionOnlyAssemblyResolve += (sender, args) =>
            {
                return Assembly.ReflectionOnlyLoad(args.Name);
            };



            foreach (string dllFile in dllFileNames)
            {
                if (dllFile.ToLower().Contains("interface"))
                    continue;

                var assembly = Assembly.ReflectionOnlyLoadFrom(dllFile);
                if (assembly != null)
                {
                    Type[] types = assembly.GetTypes();
                    foreach (Type type in types)
                    {
                        if (type.IsInterface || type.IsAbstract)
                        {
                            continue;
                        }
                        else
                        {
                            if (type.GetInterface(typeof(IPlugin).FullName) != null)
                            {
                                AppDomain domain = AppDomain.CreateDomain(type.FullName);
                                var handle = domain.CreateInstanceFrom(dllFile, type.FullName);
                                var ob = (IPlugin)handle.Unwrap();

                                if (RemotingServices.IsTransparentProxy(ob))
                                {
                                    Trace.WriteLine(ob.GetName() + " loaded");
                                    //ob.ImageSaved += ob_ImageSaved;
                                    plugins.Add(ob);
                                }
                            }
                        }
                    }
                }
            }
            Trace.Unindent();
            Trace.WriteLine(DateTime.Now + " - " + plugins.Count + " plug-ins loaded.");

            return plugins.Count;
        }

        static void ob_ImageSaved(object sender, EventArgs e)
        {
            var icon = new NotifyIcon();
            icon.Icon = new Icon("icon.ico");

            icon.Visible = true;

            icon.BalloonTipTitle = "Information";
            icon.BalloonTipText = "Image saved!";
            icon.ShowBalloonTip(1000);
        }
    }
}
