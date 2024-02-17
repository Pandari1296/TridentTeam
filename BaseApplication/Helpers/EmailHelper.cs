using System.Net.Mail;
using System.Net;

namespace BaseApplication.Helpers
{
    public interface IEmailHelper
    {
        bool SendEmail(string to, string subject, string body);
    }
    public class EmailHelper : IEmailHelper
    {
        private readonly string smtpServer;
        private readonly int port;
        private readonly string username;
        private readonly string password;

        public EmailHelper(IConfiguration configuration)
        {
            var smtpSettings = configuration.GetSection("SmtpSettings");

            smtpServer = smtpSettings["Server"];
            port = int.Parse(smtpSettings["Port"]);
            username = smtpSettings["Username"];
            password = smtpSettings["Password"];
        }

        public bool SendEmail(string to, string subject, string body)
        {
            bool isEmailSent = false;
            using (MailMessage mail = new MailMessage(username, to))
            {
                mail.Subject = subject;
                mail.Body = body;
                mail.IsBodyHtml = true;

                using (SmtpClient smtpClient = new SmtpClient(smtpServer))
                {
                    smtpClient.Port = port;
                    //smtpClient.UseDefaultCredentials = true;
                    smtpClient.Credentials = new NetworkCredential(username, password);
                    smtpClient.EnableSsl = true;
                    smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;

                    try
                    {
                        smtpClient.Send(mail);
                        isEmailSent = true;
                    }
                    catch (Exception ex)
                    {
                        isEmailSent = false;
                    }
                }
            }
            return isEmailSent;
        }
    }
}
