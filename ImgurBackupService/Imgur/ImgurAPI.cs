using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ImgurBackupService.Imgur
{
    public class ImgurAPI
    {
        public string RESOURCE_URL_IMAGE;
        public string RESOURCE_URL_ALBUM;
        public string CLIENT_ID;

        public ImgurAPI()
        {
            RESOURCE_URL_IMAGE = ConfigurationManager.AppSettings["ResourceURLImage"];
            RESOURCE_URL_ALBUM = ConfigurationManager.AppSettings["ResourceURLAlbum"];
            CLIENT_ID = ConfigurationManager.AppSettings["ClientID"];
        }

        public void CreateAlbum()
        {
            try
            {
                using (WebClient client = new WebClient())
                {
                    client.Headers.Add("Authorization", "Client-ID " + CLIENT_ID);
                    byte[] response = client.UploadValues(RESOURCE_URL_ALBUM, new NameValueCollection()
                    {
                        { "title", "ScreenShoter" },
                        { "privacy", "secret" },
                    });
                    string json = System.Text.Encoding.UTF8.GetString(response);

                    JObject ob = JObject.Parse(json);

                    ConfigurationManager.AppSettings["AlbumID"] = (string)ob["data"]["id"];
                    ConfigurationManager.AppSettings["AlbumDeleteHash"] = (string)ob["data"]["deletehash"];

                    Trace.WriteLine(json);
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message + ex.StackTrace);
            }
        }

        public void DeleteAlbum()
        {
            try
            {
                using (WebClient client = new WebClient())
                {
                    client.Headers.Add("Authorization", "Client-ID " + CLIENT_ID);
                    var res = client.UploadString(RESOURCE_URL_ALBUM + "/" + ConfigurationManager.AppSettings["AlbumDeleteHash"], "DELETE", "");
                    Trace.WriteLine(res);
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message + ex.StackTrace);
            }
        }

        public void SaveImage(byte[] image)
        {
            // Convert byte[] to Base64 String
            string base64String = Convert.ToBase64String(image);
            using (WebClient client = new WebClient())
            {
                client.Headers.Add("Authorization", "Client-ID " + CLIENT_ID);
                byte[] response = client.UploadValues(RESOURCE_URL_IMAGE, new NameValueCollection()
                    {
                        { "image", base64String },
                        { "title", "ScreenShoter" }
                    });
                string json = System.Text.Encoding.UTF8.GetString(response);

                JObject o = JObject.Parse(json);

                Trace.WriteLine(DateTime.Now + " - Image uploaded: " + (string)o["data"]["link"]);
            }
        }
    }
}
