using PluginInterface;
using ScreenShoter.Assets.Temp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ScreenShoter
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo(ConfigurationManager.AppSettings["Culture"]);

            PluginResolver.LoadPluginsFromDirectory(ConfigurationManager.AppSettings["PluginDir"]);

            Trace.WriteLine(DateTime.Now + " - Application started");
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
