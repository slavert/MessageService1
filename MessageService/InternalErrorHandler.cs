using System;
using System.ServiceModel.Dispatcher;

namespace MessageService
{ 
    //Input message internal error handler
    public class InternalErrorHandler : IErrorHandler
    {

        public bool HandleError(Exception error)
        {
            IDatabaseConnection databaseConnection = new DatabaseConnectionMSSQL();
            MessageResponse messageResponse = new MessageResponse() 
            { 
                ErrorMessage = error.Message,
                ReturnCode = ReturnCode.InternalError 
            };
            databaseConnection.WriteToDatabase(null, null, ref messageResponse);
            return false;
        }

        public void ProvideFault(Exception error, System.ServiceModel.Channels.MessageVersion version, ref System.ServiceModel.Channels.Message fault)
        {
        }
    }
}