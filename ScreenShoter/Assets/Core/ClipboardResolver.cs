using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ScreenShoter.Assets.Core
{
    class ClipboardResolver
    {
        public static void AddToClipboard(Object o)
        {
            Trace.WriteLine(DateTime.Now + " - Saved image to clipboard");
            Clipboard.SetDataObject(o);
        }
    }
}
