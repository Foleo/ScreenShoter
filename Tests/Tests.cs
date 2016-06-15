using ClipboardImageSaver;
using JpegImageSaver;
using NUnit.Framework;
using ScreenShoter.Assets.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PngImageSaver;
using ScreenShoter.Assets.Temp;

namespace Tests
{
    [TestFixture]
    public class Tests
    {
        public static IEnumerable PointSource
        {
            get
            {
                List<Point> points = new List<Point>();

                points.Add(new Point(-10, 10));
                points.Add(new Point(10, 10));
                points.Add(new Point(0, -5));
                points.Add(new Point(14, 0));

                return points;
            }
        }

        public static IEnumerable SizeSource
        {
            get
            {
                List<Size> sizes = new List<Size>();

                sizes.Add(new Size(-20, 200));
                sizes.Add(new Size(1920, 0));
                sizes.Add(new Size(200, 200));
                sizes.Add(new Size(1920, 1024));

                return sizes;
            }
        }

        public static IEnumerable ExpectedSource
        {
            get
            {
                List<bool> exp = new List<bool>();

                exp.Add(true);
                exp.Add(true);
                exp.Add(true);
                exp.Add(false);

                return exp;
            }
        }

        public static IEnumerable ExpectedSource2
        {
            get
            {
                List<int> exp = new List<int>();

                exp.Add(4);
                exp.Add(0);
                exp.Add(0);
                exp.Add(1);

                return exp;
            }
        }

        public static IEnumerable ExpectedSource3
        {
            get
            {
                List<bool> exp = new List<bool>();

                exp.Add(true);
                exp.Add(false);
                exp.Add(true);

                return exp;
            }
        }

        public static IEnumerable PathSource
        {
            get
            {
                List<string> paths = new List<string>();

                paths.Add(@"C:\Users\Filip\OneDrive\Work\Programming\C#\ScreenShoter\Tests\Assets\Plugins");
                paths.Add(@"C:\Users\Filip\OneDrive\Work\Programming\C#\ScreenShoter\Tests\Assets\Plugins\Tmp");
                paths.Add(@"C:\Users\Filip\OneDrive\Work\Programming\C#\ScreenShoter\Tests\Assets\Plugins\Java");
                paths.Add(@"C:\Users\Filip\OneDrive\Work\Programming\C#\ScreenShoter\Tests\Assets\Plugins\Fake");

                return paths;
            }
        }

        public static IEnumerable PathSource2
        {
            get
            {
                List<string> exp = new List<string>();

                exp.Add(@"C:\Users\Filip\OneDrive\Work\Programming\C#\ScreenShoter\Tests\Assets\Images");
                exp.Add("");
                exp.Add(@"C:\Users\Filip\OneDrive\Work\Programming\C#\ScreenShoter\Tests\Assets\Images");

                return exp;
            }
        }

        public static IEnumerable ImageSource
        {
            get
            {
                List<byte[]> images = new List<byte[]>();
                List<Image> input = new List<Image>();

                input.Add(Image.FromFile(@"C:\Users\Filip\OneDrive\Work\Programming\C#\ScreenShoter\Tests\Assets\Images\logo.gif"));
                input.Add(Image.FromFile(@"C:\Users\Filip\OneDrive\Work\Programming\C#\ScreenShoter\Tests\Assets\Images\logo.gif"));
                input.Add(Image.FromFile(@"C:\Users\Filip\OneDrive\Work\Programming\C#\ScreenShoter\Tests\Assets\Images\logo.gif"));

                foreach (Image im in input)
                {
                    using (var ms = new MemoryStream())
                    {
                        im.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                        images.Add(ms.ToArray());
                    }                    
                }

                return images;
            }
        }

        public static IEnumerable EventSource
        {
            get
            {
                List<EventHandler> evs = new List<EventHandler>();

                evs.Add(null);
                evs.Add(null);
                evs.Add(Ev);

                return evs;
            }
        }


        /* ID: #1
         * NAME: Capture screen boundary test
         * DESCRIPTION: Tests method that captures part of the screen with input size on input location.
         * PREREQUISITES: - 
         * ACTIONS:
         *  1. Move the window to desired location
         *  2. Resize the window to desired size
         *  3. Double click the window
         * EXPECTED RESULT: Method returns an image which corresponds to captured screen.
         */
        [Test]
        [Sequential]
        public static void CaptureTest(
            [ValueSource("PointSource")] Point p,
            [ValueSource("SizeSource")] Size s,
            [ValueSource("ExpectedSource")] bool e)
        {
            Image b = CaptureResolver.CaptureBoundaries(p, s);

            Assert.AreEqual(e, b == null);
        }


        /* ID: #2
         * NAME: Plug-in load test
         * DESCRIPTION: Tests whether all plugins are correctly loaded
         * PREREQUISITES: Plug-ins are installed in Plugin folder
         * ACTIONS:
         *  1. Start the application
         * EXPECTED RESULT: Plug-ins are loaded in application memory and display in context menu
         */
        [Test]
        [Sequential]
        public static void PluginTest([ValueSource("PathSource")]string path,
            [ValueSource("ExpectedSource2")] int e)
        {
            int cnt = PluginResolver.LoadPluginsFromDirectory(path);
            Assert.AreEqual(PluginResolver.GetPlugins().Count, cnt);
        }


        /* ID: #3
         * NAME: Save image to Png
         * DESCRIPTION: Test whether input byte array correctly saves to disc as PNG file.
         * PREREQUISITES: - 
         * ACTIONS:
         *  1. Set the output method to PNG
         *  2. Enter correct output directory in options
         *  3. Capture the screen
         * EXPECTED RESULT: Image is saved to output directory.
         */
        [Test]
        [Sequential]
        public static void PngSaveTest([ValueSource("ImageSource")] byte[] image,
            [ValueSource("PathSource2")] string path,
            [ValueSource("EventSource")] EventHandler events,
            [ValueSource("ExpectedSource3")] bool e)
        {
            PngResolver resolver = new PngResolver();
            PngResolver.DIR_PATH = path;

            resolver.ImageSaved += events;

            string res = resolver.SaveImage(image);
            Assert.AreEqual(e, File.Exists(res));
        }

        public static void Ev(object o, EventArgs e)
        {
            
        }
    }
}
