using System;
using System.Configuration;
using System.Net.Mail;
using System.Text;

namespace MessageService
{
    //Sending requested message to recipient
    public class EmailSender : ISender
    {
        IDatabaseConnection DatabaseConnection { get; set; }

        public EmailSender(IDatabaseConnection databaseConnection)
        {
            DatabaseConnection = databaseConnection;
        }

        public void SendMessage(MessageRequest message, string address, ref MessageResponse senderMessageResponse)
        {
            //Logging actions to database
            DatabaseConnection.WriteToDatabase(message, address, ref senderMessageResponse);

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
                senderMessageResponse = new MessageResponse()
                {
                    ErrorMessage = error.Message,
                    ReturnCode = ReturnCode.InternalError
                };
                DatabaseConnection.WriteToDatabase(null, null, ref senderMessageResponse);
            }
            
        }
    }

    public interface ISender
    {
        void SendMessage(MessageRequest message, string address, ref MessageResponse senderMessageResponse);
    }
}