using PluginInterface;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ClipboardImageSaver
{
    [Serializable]
    public class ClipboardResolver : MarshalByRefObject, IPlugin
    {
        public string SaveImage(byte[] image)
        {
            using (MemoryStream ms = new MemoryStream(image))
            {
                Image img = Image.FromStream(ms);
                Clipboard.SetImage(img);

                if (ImageSaved != null)
                    ImageSaved(this, EventArgs.Empty);

                return "";
            }
        }

        public string GetOutputName()
        {
            return "Copy to clipboard";
        }

        public string GetName()
        {
            return "Clipboard plug-in";
        }

        public string GetDescription()
        {
            return "Copies image to clipboard.";
        }

        public Form OptionsForm
        {
            get { return null; }
        }

        public event EventHandler ImageSaved;

        public override object InitializeLifetimeService()
        {
            return null;
        }
    }
}
