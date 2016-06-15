using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ImgurSender
{
    public partial class Options : Form
    {
        public Options()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            IsolatedStorageFile isoStore = IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Assembly, null, null);
            if (isoStore.FileExists("config"))
            {
                using (IsolatedStorageFileStream isoStream = new IsolatedStorageFileStream("config", FileMode.Create, isoStore))
                {
                    using (StreamWriter writer = new StreamWriter(isoStream))
                    {
                        writer.Write(JsonConvert.SerializeObject(new { API_KEY = textBox1.Text.Trim(), UPLOAD_URL = textBox2.Text.Trim() }));
                    }
                }

                using (IsolatedStorageFileStream isoStream = new IsolatedStorageFileStream("config", FileMode.Open, isoStore))
                {
                    using (StreamReader reader = new StreamReader(isoStream))
                    {
                        var str = reader.ReadToEnd();
                        var obj = JObject.Parse(str);
                        ImgurResolver.API_KEY = (string)obj.GetValue("API_KEY");
                        ImgurResolver.UPLOAD_URL = (string)obj.GetValue("UPLOAD_URL");
                    }
                }
            }

            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Options_Load(object sender, EventArgs e)
        {
            textBox1.Text = ImgurResolver.API_KEY;
            textBox2.Text = ImgurResolver.UPLOAD_URL;
        }
    }
}
