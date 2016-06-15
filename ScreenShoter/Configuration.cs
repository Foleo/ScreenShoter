using ScreenShoter.Assets.Temp;
using System;
using System.Configuration;
using System.Drawing;
using System.Globalization;
using System.Threading;
using System.Windows.Forms;

namespace ScreenShoter
{
    public partial class Configures : Form
    {
        public Configures()
        {
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(ConfigurationManager.AppSettings["Culture"]);
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult result = colorDialog1.ShowDialog();
            label2.BackColor = colorDialog1.Color;
        }

        private void label2_Click(object sender, EventArgs e)
        {
            DialogResult result = colorDialog1.ShowDialog();

            if (result == DialogResult.OK)
            {
                label2.BackColor = colorDialog1.Color;
            }
                
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            DialogResult result = folderBrowserDialog1.ShowDialog();

            if (result == DialogResult.OK)
                textBox1.Text = folderBrowserDialog1.SelectedPath;
        }

        private void label2_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawRectangle(new Pen(Color.Black), 0, 0, label2.Width - 1, label2.Height - 1);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            System.Configuration.Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            config.AppSettings.Settings["Destination"].Value = textBox1.Text;
            config.AppSettings.Settings["UIColorR"].Value = label2.BackColor.R.ToString();
            config.AppSettings.Settings["UIColorG"].Value = label2.BackColor.G.ToString();
            config.AppSettings.Settings["UIColorB"].Value = label2.BackColor.B.ToString();
            config.AppSettings.Settings["BGColorR"].Value = label8.BackColor.R.ToString();
            config.AppSettings.Settings["BGColorG"].Value = label8.BackColor.G.ToString();
            config.AppSettings.Settings["BGColorB"].Value = label8.BackColor.B.ToString();
            config.AppSettings.Settings["BGOpacity"].Value = trackBar1.Value * 10 + "";
            config.Save(ConfigurationSaveMode.Modified);

            ConfigurationManager.RefreshSection("appSettings");

            this.DialogResult = DialogResult.OK;
            this.Hide();  
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Hide();
        }

        private void Configures_Load(object sender, EventArgs e)
        {
            textBox1.Text = ConfigurationManager.AppSettings["Destination"];
            label2.BackColor = Color.FromArgb(
                Int16.Parse(ConfigurationManager.AppSettings["UIColorA"]),
                Int16.Parse(ConfigurationManager.AppSettings["UIColorR"]),
                Int16.Parse(ConfigurationManager.AppSettings["UIColorG"]),
                Int16.Parse(ConfigurationManager.AppSettings["UIColorB"]));
            label8.BackColor = Color.FromArgb(
                Int16.Parse(ConfigurationManager.AppSettings["BGColorR"]),
                Int16.Parse(ConfigurationManager.AppSettings["BGColorG"]),
                Int16.Parse(ConfigurationManager.AppSettings["BGColorB"]));
            trackBar1.Value = Int16.Parse(ConfigurationManager.AppSettings["BGOpacity"]) / 10;
            label10.Text = ConfigurationManager.AppSettings["BGOpacity"] + "%";

            

            foreach (var p in PluginResolver.GetPlugins())
            {
                Form optionForm = p.OptionsForm;

                if (optionForm != null)
                {
                    var butt = new Button();
                    butt.Text = p.GetName() + " options";
                    butt.AutoSize = true;

                    butt.Click += delegate 
                    {
                        optionForm.ShowDialog();
                    };

                    panel1.Controls.Add(butt);
                }
            }
            
        }

        private void label8_Click(object sender, EventArgs e)
        {
            DialogResult result = colorDialog2.ShowDialog();

            if (result == DialogResult.OK)
            {
                label8.BackColor = colorDialog2.Color;
            }
        }

        private void label8_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawRectangle(new Pen(Color.Black), 0, 0, label8.Width - 1, label8.Height - 1);
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            label10.Text = trackBar1.Value + "0%";
        }
    }
}
