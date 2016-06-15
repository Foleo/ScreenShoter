using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PngImageSaver
{
    public partial class Options : Form
    {
        public Options()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                textBox1.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        private void Options_Load(object sender, EventArgs e)
        {
            textBox1.Text = PngResolver.DIR_PATH;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            IsolatedStorageFile isoStore = IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Assembly, null, null);

            using (IsolatedStorageFileStream isoStream = new IsolatedStorageFileStream("config", FileMode.Create, isoStore))
            {
                using (StreamWriter writer = new StreamWriter(isoStream))
                {
                    writer.WriteLine(textBox1.Text.Trim());
                }
            }

            using (IsolatedStorageFileStream isoStream = new IsolatedStorageFileStream("config", FileMode.Open, isoStore))
            {
                using (StreamReader reader = new StreamReader(isoStream))
                {
                    string dir = reader.ReadToEnd();
                    PngResolver.DIR_PATH = dir.Trim();
                }
            }

            this.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
