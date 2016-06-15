using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace ScreenShoter.Resources
{
    public static class Resources
    {
        private static ResourceManager resources = new ResourceManager("ScreenShoter.Resources.lang", Assembly.GetExecutingAssembly());
        private static CultureInfo currentCulture = CultureInfo.CurrentCulture;

        public static string GetString(string name)
        {
            return resources.GetString(name, currentCulture);
        }
    }
}
