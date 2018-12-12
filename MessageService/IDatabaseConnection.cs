using System;

namespace MessageService
{
    //Connection to database using Entity Framework
    public class DatabaseConnectionMSSQL : IDatabaseConnection
    {

        public void WriteToDatabase(MessageRequest message, string address, MessageResponse messageResponse)
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
                //Logging error when issue with database
                ExceptionLogger.Log(ReturnCode.InternalError, "Connection to database issue: " + error.Message);
            }
            
        }

    }

    public interface IDatabaseConnection
    {
        void WriteToDatabase(MessageRequest message, string address, MessageResponse messageResponse);
    }
}