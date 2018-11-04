using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MessageService
{
    public class DatabaseConnectionMSSQL : IDatabaseConnection
    {
        public void WriteToDatabase(MessageRequest message, string address) 
        {
            using (var db = new MessageLogContext())
            {
                var MessageLog = new MessageLog()
                {
                    EmailAddress = address,
                    Subject = message.Subject,
                    Content = message.Message,
                    CreationDate = DateTime.Now
                };

                db.MessageLogDbSet.Add(MessageLog);
                db.SaveChanges();
            }
        }

        public void WriteToDatabase(MessageRequest message, string address, string errorMessage, string returnCode)
        {
            using (var db = new MessageLogContext())
            {
                var MessageLog = new MessageLog()
                {
                    EmailAddress = address,
                    Subject = message.Subject,
                    Content = message.Message,
                    ErrorMessage = errorMessage,
                    ReturnCode = returnCode,
                    CreationDate = DateTime.Now
                };

                db.MessageLogDbSet.Add(MessageLog);
                db.SaveChanges();
            }
        }
    }

    public interface IDatabaseConnection
    {
        void WriteToDatabase(MessageRequest message, string address);
        void WriteToDatabase(MessageRequest message, string address, string errorMessage, string returnCode);
    }
}