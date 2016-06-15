using Newtonsoft.Json.Linq;
using System;
using System.Configuration;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using ScreenShoter.Assets.Core;
using ScreenShoter.Resources;
using System.Drawing.Drawing2D;
using ScreenShoter.Assets.Temp;

namespace ScreenShoter
{
    public partial class Form1 : Form
    {
        private bool dragging = false;
        private Point dragCursorPoint;
        private Point dragFormPoint;

        public NotifyIcon icon;

        private const int cGrip = 16;      
        private const int cCaption = 32;

        MenuItem output;

        Form f2;
        public Form1()
        {
            InitializeComponent();

            f2 = new Form2();
            f2.StartPosition = FormStartPosition.Manual;
            f2.Location = new Point(200, 200);
            f2.Show();
            this.BackColor = Color.Red;
            this.TransparencyKey = Color.Red;
            this.StartPosition = FormStartPosition.Manual;
            this.Location = new Point(200, 200);

            this.LocationChanged += new EventHandler(Form1_LocationChanged);
            this.SizeChanged += new EventHandler(Form1_SizeChanged);
            this.FormClosed += new FormClosedEventHandler(Form1_FormClosed);

            icon = new NotifyIcon();
            icon.Icon = new Icon("icon.ico");
            icon.MouseUp += new MouseEventHandler(Icon_Clicked);
            icon.Visible = true;

            InitializeMenu();

            this.MouseWheel += new MouseEventHandler(Form1_Scrolling);
        }

        private void Form1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.Hide();
            f2.Hide();
            System.Threading.Thread.Sleep(250);
            CaptureResolver.ProcessImage(CaptureResolver.CaptureBoundaries(this.Location, this.Size));
            this.Show();
            f2.Show();

            f2.TopMost = true;
            this.TopMost = true;
            f2.TopMost = false;
            this.TopMost = false;
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                dragging = true;
                dragCursorPoint = Cursor.Position;
                dragFormPoint = this.Location;
            }
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            if (dragging)
            {
                Point dif = Point.Subtract(Cursor.Position, new Size(dragCursorPoint));
                this.Location = Point.Add(dragFormPoint, new Size(dif));
            }
        }

        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            dragging = false;
        }

        private void Form1_MouseDown_1(object sender, MouseEventArgs e)
        {

        }

        private void Form1_Scrolling(object sender, MouseEventArgs e)
        {
            this.Width += e.Delta / 10;
            this.Height += e.Delta / 10;
        }

        private void Icon_Clicked(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                this.WindowState = FormWindowState.Normal;
                f2.TopMost = true;
                this.TopMost = true;
                f2.TopMost = false;
                this.TopMost = false;
            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            f2.Close();
            icon.Icon = null;
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            int R = Int16.Parse(ConfigurationManager.AppSettings["UIColorR"]);
            int G = Int16.Parse(ConfigurationManager.AppSettings["UIColorG"]);
            int B = Int16.Parse(ConfigurationManager.AppSettings["UIColorB"]);
            int A = Int16.Parse(ConfigurationManager.AppSettings["UIColorA"]);
            Color UIcolor = Color.FromArgb(A, R, G, B);

            e.Graphics.DrawRectangle(new Pen(UIcolor), 0, 0, this.Width - 1, this.Height - 1);

            e.Graphics.DrawLine(new Pen(UIcolor), new Point(this.Width / 2 - 25, this.Height / 2), new Point(this.Width / 2 + 25, this.Height / 2));
            e.Graphics.DrawLine(new Pen(UIcolor), new Point(this.Width / 2, this.Height / 2 - 25), new Point(this.Width / 2, this.Height / 2 + 25));

            e.Graphics.DrawLine(new Pen(UIcolor), new Point(this.Width - 5, this.Height - 3), new Point(this.Width - 3, this.Height - 5));
            e.Graphics.DrawLine(new Pen(UIcolor), new Point(this.Width - 10, this.Height - 3), new Point(this.Width - 3, this.Height - 10));
            e.Graphics.DrawLine(new Pen(UIcolor), new Point(this.Width - 15, this.Height - 3), new Point(this.Width - 3, this.Height - 15));

        }

        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            f2.Size = this.Size;
            this.Refresh();
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == 0x84)
            {  
                Point pos = new Point(m.LParam.ToInt32() & 0xffff, m.LParam.ToInt32() >> 16);
                pos = this.PointToClient(pos);
                if (pos.Y < cCaption)
                {
                    m.Result = (IntPtr)2; 
                    return;
                }
                if (pos.X >= this.ClientSize.Width - cGrip && pos.Y >= this.ClientSize.Height - cGrip)
                {
                    m.Result = (IntPtr)17;
                    return;
                }
            }
            base.WndProc(ref m);
        }

        private void InitializeMenu()
        {
            ContextMenu contextMenu = new ContextMenu();

            output = new MenuItem(Resources.Resources.GetString("OutputText"));

            foreach (var plugin in PluginResolver.GetPlugins())
            {
                MenuItem item = new MenuItem();
                item.Text = plugin.GetOutputName();

                item.Click += delegate
                {
                    foreach (var it in output.MenuItems)
                    {
                        if (it != item)
                        {
                            MenuItem i = (MenuItem)it;
                            i.Checked = false;
                        }
                    }

                    item.Checked = true;

                    ConfigResolver.ChangeOutput(plugin.GetHashCode().ToString());
                    label1.Text = plugin.GetOutputName();
                };

                output.MenuItems.Add(item);
            }

            contextMenu.MenuItems.Add(output);
            contextMenu.MenuItems.Add(Resources.Resources.GetString("OptionsText"));
            contextMenu.MenuItems.Add(Resources.Resources.GetString("BrowseText"));
            contextMenu.MenuItems.Add("-");
            contextMenu.MenuItems.Add(Resources.Resources.GetString("HideText"));
            contextMenu.MenuItems.Add("-");
            contextMenu.MenuItems.Add(Resources.Resources.GetString("ExitText"));

            contextMenu.MenuItems[1].Click += delegate 
            {
                var OptionsForm = new Configures();
                if (OptionsForm.ShowDialog() == DialogResult.OK)
                {
                    this.Refresh();
                    f2.Refresh();

                    f2.TopMost = true;
                    this.TopMost = true;
                    f2.TopMost = false;
                    this.TopMost = false;
                    
                }
                    
            };

            contextMenu.MenuItems[2].Click += delegate
            {
                string windir = Environment.GetEnvironmentVariable("WINDIR");
                System.Diagnostics.Process prc = new System.Diagnostics.Process();
                prc.StartInfo.FileName = windir + @"\explorer.exe";
                prc.StartInfo.Arguments = ConfigurationManager.AppSettings["Destination"];
                prc.Start();
            };

            contextMenu.MenuItems[4].Click += delegate
            {
                this.WindowState = FormWindowState.Minimized;
            };

            contextMenu.MenuItems[6].Click += delegate { Application.Exit(); };

            this.ContextMenu = contextMenu;
            icon.ContextMenu = contextMenu;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            string outputmethod = ConfigurationManager.AppSettings["OutputMethod"];

            var plugin = PluginResolver.GetPlugin(outputmethod);
            if (plugin != null)
            {
                label1.Text = plugin.GetOutputName();
                label1.BackColor = Color.FromArgb(
                    Int16.Parse(ConfigurationManager.AppSettings["UIColorR"]),
                    Int16.Parse(ConfigurationManager.AppSettings["UIColorG"]),
                    Int16.Parse(ConfigurationManager.AppSettings["UIColorB"]));
            }
            else
            {
                label1.Text = "-";
                label1.BackColor = Color.FromArgb(
                    Int16.Parse(ConfigurationManager.AppSettings["UIColorR"]),
                    Int16.Parse(ConfigurationManager.AppSettings["UIColorG"]),
                    Int16.Parse(ConfigurationManager.AppSettings["UIColorB"]));
            }
            
        }

        private void Form1_LocationChanged(object sender, EventArgs e)
        {
            f2.Location = this.Location;
        }
    }
}
