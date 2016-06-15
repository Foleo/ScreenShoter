using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PluginInterface
{
    public interface IPlugin
    {
        Form OptionsForm { get; }

        event EventHandler ImageSaved;

        string SaveImage(byte[] image);
        string GetOutputName();
        string GetName();
        string GetDescription();
    }
}
