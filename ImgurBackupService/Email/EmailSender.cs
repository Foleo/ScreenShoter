using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace ImgurBackupService.Email
{
    public class EmailSender
    {
        public string SMTP_Server { get; set; }
        public string From_Email { get; set; }

        public EmailSender()
        {
            SMTP_Server = ConfigurationManager.AppSettings["EmailSMTPServer"];
            From_Email = ConfigurationManager.AppSettings["EmailFrom"];
        }

        public void SendMail(string to, string subject, string body, List<string> attachments = null)
        {
            Trace.WriteLine("Sending mail with " + attachments.Count + " attachments");
            try
            {
                using (SmtpClient client = new SmtpClient(SMTP_Server))
                using (MailMessage mess = new MailMessage(From_Email, to))
                {
                    mess.Subject = subject;
                    mess.Body = body.ToString();

                    foreach (string att in attachments)
                    {
                        Attachment attachment = new Attachment(att);
                        mess.Attachments.Add(attachment);
                    }

                    client.Send(mess);
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("Error when sending email: " + ex.Message + ex.StackTrace);
            }
        }
    }
}
