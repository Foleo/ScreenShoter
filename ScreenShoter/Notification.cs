using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ScreenShoter
{
    public partial class Notification : Form
    {
        private Timer timer;
        private Timer closeTimer;
        private int startPosX;
        private int startPosY;

        public Notification()
        {
            InitializeComponent();
            // We want our window to be the top most
            TopMost = true;
            // Pop doesn't need to be shown in task bar
            ShowInTaskbar = false;
            // Create and run timer for animation
            timer = new Timer();
            timer.Interval = 50;
            timer.Tick += timer_Tick;

            closeTimer = new Timer();
            closeTimer.Interval = 5000;
            closeTimer.Tick += closeTimer_Tick;
        }

        protected override void OnLoad(EventArgs e)
        {
            // Move window out of screen
            startPosX = Screen.PrimaryScreen.WorkingArea.Width - Width;
            startPosY = Screen.PrimaryScreen.WorkingArea.Height;
            SetDesktopLocation(startPosX, startPosY);
            base.OnLoad(e);
            // Begin animation
            timer.Start();
            closeTimer.Start();
        }

        void timer_Tick(object sender, EventArgs e)
        {
            //Lift window by 5 pixels
            startPosY -= 5;
            //If window is fully visible stop the timer
            if (startPosY < Screen.PrimaryScreen.WorkingArea.Height - Height)
            {
                timer.Stop();
            }
            else
                SetDesktopLocation(startPosX, startPosY);
        }

        void closeTimer_Tick(object sender, EventArgs e)
        {
            closeTimer.Stop();
            this.Close();
        }
    }
}
