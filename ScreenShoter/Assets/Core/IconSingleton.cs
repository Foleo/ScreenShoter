using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ScreenShoter.Assets.Core
{
    static class IconSingleton
    {
        private static NotifyIcon instance;

        public static NotifyIcon Icon
        { 
            get
            {
                if (instance == null)
                {
                    instance = new NotifyIcon();
                    instance.Icon = new Icon("../../icon.ico");
                    instance.Visible = true;
                }

                return instance;
            }
        }
    }
}
