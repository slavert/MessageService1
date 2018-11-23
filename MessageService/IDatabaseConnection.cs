using System;

namespace MessageService
{
    //Connection to database using Entity Framework
    public class DatabaseConnectionMSSQL : IDatabaseConnection
    {

        public void WriteToDatabase(MessageRequest message, string address, string errorMessage, string returnCode)
        {
            using (var db = new MessageLogContext())
            {
                var MessageLog = new MessageLog()
                {
                    EmailAddress = address,
                    ErrorMessage = errorMessage,
                    ReturnCode = returnCode,
                    CreationDate = DateTime.Now
                };

                if (message!=null)
                {
                    MessageLog.Subject = message.Subject;
                    MessageLog.Content = message.Message;
                }

                db.MessageLogDbSet.Add(MessageLog);
                db.SaveChanges();
            }
        }

    }

    public interface IDatabaseConnection
    {
        void WriteToDatabase(MessageRequest message, string address, string errorMessage, string returnCode);
    }
}