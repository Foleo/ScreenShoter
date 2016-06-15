using Newtonsoft.Json.Linq;
using ScreenShoter.Assets.Temp;
using System;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace ScreenShoter.Assets.Core
{
    public class CaptureResolver
    {
        public static Image CaptureScreen()
        {
            Trace.WriteLine(DateTime.Now + " - Capturing screen");

            Bitmap bitmap = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height, PixelFormat.Format32bppArgb);

            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                graphics.CopyFromScreen(Screen.PrimaryScreen.Bounds.X, Screen.PrimaryScreen.Bounds.Y, 0, 0, Screen.PrimaryScreen.Bounds.Size, CopyPixelOperation.SourceCopy);
            }

            Trace.WriteLine(DateTime.Now + " - Captured full screen");
            return bitmap;
        }

        public static Image CaptureBoundaries(Point location, Size s)
        {
            Trace.WriteLine(DateTime.Now + " - Capturing screen");

            if (location.X >= 0 && location.Y >= 0)
            {
                if (s.Height > 0 && s.Width > 0)
                {

                    Bitmap bitmap = new Bitmap(s.Width, s.Height, PixelFormat.Format32bppArgb);

                    using (Graphics graphics = Graphics.FromImage(bitmap))
                    {
                        graphics.CopyFromScreen(location.X, location.Y, 0, 0, bitmap.Size, CopyPixelOperation.SourceCopy);
                    }

                    Trace.WriteLine(DateTime.Now + " - Captured " + s.Width + "x" + s.Height + " boundaries");

                    return bitmap;
                }
                else
                {
                    Trace.WriteLine("Invalid location");
                    return null;
                }

            }
            else
            {
                Trace.WriteLine("Invalid location");
                return null;
            }
        }

        public static void ProcessImage(Image image)
        {
            Trace.WriteLine(DateTime.Now + " - Processing image");

            Thread thr = new Thread(ProcessImageThread);
            thr.SetApartmentState(ApartmentState.STA);
            thr.IsBackground = true;
            thr.Start(image);
        }

        static void ProcessImageThread(object img)
        {
            byte[] image;

            using (MemoryStream ms = new MemoryStream())
            {
                var pic = (Image)img;
                pic.Save(ms, ImageFormat.Png);
                image = ms.ToArray();
            }

            string method = ConfigurationManager.AppSettings["OutputMethod"];
            try
            {
                var plugin = PluginResolver.GetPlugin(method);

                plugin.SaveImage(image);

                Trace.WriteLine(DateTime.Now + " - Image processed");
            }
            catch (Exception e)
            {
                MessageBox.Show(Resources.Resources.GetString("ErrorProcessingImage"), Resources.Resources.GetString("ErrorText"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                Trace.WriteLine(DateTime.Now + " - Error processing image: " + e.ToString());
            }
        }
    }
}
