using Newtonsoft.Json;
using NUnit.Framework;
using PluginInterface;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JpegImageSaver
{
    [Serializable]
    public class JpegResolver : MarshalByRefObject, IPlugin
    {
        public static string DIR_PATH { get; set; }

        public JpegResolver() 
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
                    while (File.Exists(DIR_PATH + "\\ScreenShot" + i + ".jpg"))
                    {
                        i++;
                    }

                    Image img = Image.FromStream(ms);

                    img.Save(DIR_PATH + "\\ScreenShot" + i + ".jpg", ImageFormat.Jpeg);

                    Clipboard.SetText(DIR_PATH + "\\ScreenShot" + i + ".jpg");

                    if (ImageSaved != null)
                        ImageSaved(this, EventArgs.Empty);

                    return DIR_PATH + "\\ScreenShot" + i + ".jpg";
                }
            }
            else
            {
                MessageBox.Show("File path is empty");
                return null;
            }
        }

        public string GetName()
        {
            return "Jpeg plug-in";
        }

        public string GetOutputName()
        {
            return "Save to JPEG";
        }

        public string GetDescription()
        {
            return "Saves image in Jpeg format.";
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
