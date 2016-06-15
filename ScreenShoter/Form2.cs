using System;
using System.Configuration;
using System.Drawing;
using System.Windows.Forms;

namespace ScreenShoter
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
            this.BackColor = Color.FromArgb(
                Int16.Parse(ConfigurationManager.AppSettings["BGColorR"]),
                Int16.Parse(ConfigurationManager.AppSettings["BGColorG"]),
                Int16.Parse(ConfigurationManager.AppSettings["BGColorB"]));

            this.Opacity = Int16.Parse(ConfigurationManager.AppSettings["BGOpacity"]) * 0.01;
        }

        private void Form2_Paint(object sender, PaintEventArgs e)
        {
            this.BackColor = Color.FromArgb(
                Int16.Parse(ConfigurationManager.AppSettings["BGColorR"]),
                Int16.Parse(ConfigurationManager.AppSettings["BGColorG"]),
                Int16.Parse(ConfigurationManager.AppSettings["BGColorB"]));

            this.Opacity = Int16.Parse(ConfigurationManager.AppSettings["BGOpacity"]) * 0.01;
        }
    }
}
