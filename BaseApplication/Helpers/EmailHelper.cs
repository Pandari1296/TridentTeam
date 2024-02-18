using System.Net.Mail;
using System.Net;

namespace BaseApplication.Helpers
{
    public interface IEmailHelper
    {
        bool SendEmail(ILogger logger, string to, string subject, string body);
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

        public bool SendEmail(ILogger logger, string to, string subject, string body)
        {
            try
            {
                logger.LogInformation("EmailHelper | SendEmail | Enterd into sending email to : " + to);

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
                        smtpClient.Send(mail);
                        isEmailSent = true;
                        logger.LogInformation("EmailHelper | SendEmail | Email sent successfully!");
                    }
                }
                return isEmailSent;
            }
            catch (Exception ex)
            {
                logger.LogError("EmailHelper | SendEmail | Exception occured into : " + ex.Message);
                throw;
            }
        }
    }
}
