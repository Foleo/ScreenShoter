using Newtonsoft.Json.Linq;
using NUnit.Framework;
using PluginInterface;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ImgurSender
{
    [Serializable]
    public class ImgurResolver : MarshalByRefObject, IPlugin
    {
        static public string API_KEY { get; set; }
        static public string UPLOAD_URL { get; set; }

        public ImgurResolver()
        {
            IsolatedStorageFile isoStore = IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Assembly, null, null);

            if (isoStore.FileExists("config"))
            {
                using (IsolatedStorageFileStream isoStream = new IsolatedStorageFileStream("config", FileMode.Open, isoStore))
                {
                    using (StreamReader reader = new StreamReader(isoStream))
                    {
                        var str = reader.ReadToEnd();

                        if (str.Trim().Length != 0)
                        {
                            var obj = JObject.Parse(str);
                            API_KEY = (string)obj.GetValue("API_KEY");
                            UPLOAD_URL = (string)obj.GetValue("UPLOAD_URL");
                        }
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

        public ImgurResolver(string api_key, string upload_url)
        {
            API_KEY = api_key;
            UPLOAD_URL = upload_url;
        }

        public string SaveImage(byte[] image)
        {
            using (WebClient client = new WebClient())
            {
                try
                {
                // Convert byte[] to Base64 String
                        string base64String = Convert.ToBase64String(image);

                        client.Headers.Add("Authorization", "Client-ID " + API_KEY);
                        byte[] response = client.UploadValues(UPLOAD_URL, new NameValueCollection()
                        {
                            { "image", base64String },
                            { "title", "ScreenShoter" },
                            { "album", ConfigurationManager.AppSettings["AlbumID"]}
                        });
                        string json = System.Text.Encoding.UTF8.GetString(response);

                        JObject o = JObject.Parse(json);

                        string result = (string)o["data"]["link"];
                        Clipboard.SetText(result);

                        if (ImageSaved != null)
                            ImageSaved(this, EventArgs.Empty);

                        return result;
                    }
                    catch (WebException wex)
                    {
                        MessageBox.Show("Check your internet connection.");
                        return "";
                    }
                    catch (Exception ex)
                    {
                        return "";
                    }
                }
        }

        public string GetName()
        {
            return "Imgur plug-in";
        }

        public string GetOutputName()
        {
            return "Upload to Imgur";
        }

        public string GetDescription()
        {
            return "Uploads image to imgur.com and copies link to clipboard.";
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
