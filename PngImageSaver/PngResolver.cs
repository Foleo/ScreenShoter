using PluginInterface;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PngImageSaver
{
    [Serializable]
    public class PngResolver : MarshalByRefObject, IPlugin
    {
        public static string DIR_PATH { get; set; }

        public PngResolver()
        {
            IsolatedStorageFile isoStore = IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Assembly, null, null);

            if (isoStore.FileExists("config"))
            {
                using (IsolatedStorageFileStream isoStream = new IsolatedStorageFileStream("config", FileMode.Open, isoStore))
                {
                    using (StreamReader reader = new StreamReader(isoStream))
                    {
                        DIR_PATH = reader.ReadToEnd();
                        DIR_PATH = DIR_PATH.Trim();
                    }
                }
            }
            else
            {
                using (IsolatedStorageFileStream isoStream = new IsolatedStorageFileStream("config", FileMode.CreateNew, isoStore))
                {

                }
            }
        }

        public string SaveImage(byte[] image)
        {
            if (DIR_PATH.Trim().Length != 0)
            {
                using (MemoryStream ms = new MemoryStream(image))
                {
                    int i = 0;
                    while (File.Exists(DIR_PATH + "\\ScreenShot" + i + ".png"))
                    {
                        i++;
                    }

                    Image img = Image.FromStream(ms);

                    Trace.WriteLine(DIR_PATH + "\\ScreenShot" + i + ".png");

                    img.Save(DIR_PATH + "\\ScreenShot" + i + ".png", ImageFormat.Png);

                    if (ImageSaved != null)
                        ImageSaved(this, EventArgs.Empty);

                    return DIR_PATH + "\\ScreenShot" + i + ".png";
                }
            }
            else
            {
                MessageBox.Show("File path is empty");
                return null;
            }
        }

        public string GetOutputName()
        {
            return "Save to Png";
        }

        public string GetName()
        {
            return "Png plug-in";
        }

        public string GetDescription()
        {
            return "Saves image in Png format.";
        }

        public Form OptionsForm
        {
            get
            {
                var form = new Options();

                return form;
            }
        }

        public event EventHandler ImageSaved;

        public override object InitializeLifetimeService()
        {
            return null;
        }
    }
}
