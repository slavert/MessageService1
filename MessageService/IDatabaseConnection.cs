using System;

namespace MessageService
{
    //Connection to database using Entity Framework
    public class DatabaseConnectionMSSQL : IDatabaseConnection
    {

        public void WriteToDatabase(MessageRequest message, string address, ref MessageResponse messageResponse)
        {
            try
            {
                using (var db = new MessageLogContext())
                {
                    var MessageLog = new MessageLog()
                    {
                        EmailAddress = address,

                        CreationDate = DateTime.Now
                    };

                    if (message != null)
                    {
                        MessageLog.Subject = message.Subject;
                        MessageLog.Content = message.Message;
                    }

                    if (messageResponse != null)
                    {
                        MessageLog.ErrorMessage = messageResponse.ErrorMessage;
                        MessageLog.ReturnCode = Convert.ToString(messageResponse.ReturnCode);
                    }

                    db.MessageLogDbSet.Add(MessageLog);
                    db.SaveChanges();
                }
            }
            catch (Exception error)
            {
                if (messageResponse == null)
                    messageResponse = new MessageResponse();
                messageResponse.ErrorMessage = "Connection to database issue: " + error.Message;
                messageResponse.ReturnCode = ReturnCode.InternalError;
            }
            
        }

    }

    public interface IDatabaseConnection
    {
        void WriteToDatabase(MessageRequest message, string address, ref MessageResponse messageResponse);
    }
}