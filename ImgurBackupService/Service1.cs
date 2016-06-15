using ImgurBackupService.Email;
using ImgurBackupService.Imgur;
using ImgurBackupService.Statistics;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace ImgurBackupService
{
    public partial class Service1 : ServiceBase
    {
        public List<FileSystemWatcher> watchers;
        public System.Timers.Timer timer;
        public EmailSender emailClient;
        public ImgurAPI api;
        public StatisticsResolver stats;

        public TimeSpan statTime;

        public Service1()
        {
            InitializeComponent();

            watchers = new List<FileSystemWatcher>();
            api = new ImgurAPI();
            stats = new StatisticsResolver();

            timer = new System.Timers.Timer();
            timer.Interval = 60000;
            timer.Elapsed += timer_Elapsed;

            statTime = new TimeSpan(int.Parse(ConfigurationManager.AppSettings["StatHour"]),
                int.Parse(ConfigurationManager.AppSettings["StatMinute"]),
                0);

            emailClient = new EmailSender();
        }

        void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (DateTime.Now.TimeOfDay.Hours == statTime.Hours && DateTime.Now.TimeOfDay.Minutes == statTime.Minutes)
            {
                List<string> atts = new List<string>();
                atts.Add(stats.CreateStatistics());

                atts.ForEach(x => Trace.WriteLine(x));

                emailClient.SendMail(ConfigurationManager.AppSettings["EmailTo"], "Report", "", atts);

                stats.Clear();
            }
        }

        protected override void OnStart(string[] args)
        {
            if(ConfigurationManager.AppSettings["AlbumID"] == "0")
                api.CreateAlbum();

            string[] filters = { "*.png", "*.jpg", "*.jpeg" };
            
            foreach(string f in filters)
            {
                FileSystemWatcher w = new FileSystemWatcher();
                w.Path = ConfigurationManager.AppSettings["WatcherDIR"];
                w.Filter = f;
                w.Created += w_Created;
                watchers.Add(w);
            }

            foreach (var watcher in watchers)
            {
                watcher.EnableRaisingEvents = true;
            }

            timer.Start();
        }

        void w_Created(object sender, FileSystemEventArgs e)
        {
            Thread thr = new Thread(ProcessImageThread);
            thr.SetApartmentState(ApartmentState.STA);
            thr.IsBackground = true;
            thr.Start(e.FullPath);
        }

        private void ProcessImageThread(object obj)
        {
            Trace.WriteLine(DateTime.Now + " - New image found!");

            var filePath = (string)obj;
            var image = Image.FromFile(filePath);

            long length = new System.IO.FileInfo(filePath).Length;
            stats.AddStat(DateTime.Now, length);

            byte[] im;

            using (MemoryStream ms = new MemoryStream())
            {
                image.Save(ms, ImageFormat.Png);
                im = ms.ToArray();
            }

            if (image != null)
                api.SaveImage(im);
        }

        protected override void OnStop()
        {
            foreach (var watcher in watchers)
            {
                watcher.EnableRaisingEvents = false;
            }

            timer.Stop();
        }
    }
}
