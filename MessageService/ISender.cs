using System;
using System.Configuration;
using System.Net.Mail;
using System.Text;

namespace MessageService
{
    //Sending requested message to recipient
    public class EmailSender : ISender
    {

        public EmailSender()
        {

        }

        public void SendMessage(MessageRequest message, string address)
        {
            try
            {
                MailMessage mailMessage = new MailMessage()
                {
                    From = new MailAddress(ConfigurationManager.AppSettings["SenderAddress"]),
                    Subject = message.Subject,
                    Body = message.Message,
                    BodyEncoding = UTF8Encoding.UTF8
                };

                mailMessage.To.Add(new MailAddress(address));

                SmtpClient smtpClient = new SmtpClient()
                {
                    UseDefaultCredentials = false,
                    EnableSsl = true,
                    Host = ConfigurationManager.AppSettings["SmtpHost"],
                    Port = Convert.ToInt32(ConfigurationManager.AppSettings["SmtpPort"]),
                    Timeout = Convert.ToInt32(ConfigurationManager.AppSettings["SmtpTimeout"]),
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    Credentials = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["SmtpUsername"], ConfigurationManager.AppSettings["SmtpPassword"])
                };

                smtpClient.Send(mailMessage);
            }
            catch(Exception error)
            {
                //Logging error when issue with sending email message
                ErrorLogger.Log(ReturnCode.InternalError, error.Message);
            }
        }
    }

    public interface ISender
    {
        void SendMessage(MessageRequest message, string address);
    }
}