using System;
using System.ServiceModel.Dispatcher;

namespace MessageService
{ 
    //Internal error handler
    public class InternalErrorHandler : IErrorHandler
    {

        public bool HandleError(Exception error)
        {
            IDatabaseConnection databaseConnection = new DatabaseConnectionMSSQL();
            databaseConnection.WriteToDatabase(null, null, error.Message, Convert.ToString(ReturnCode.InternalError));
            return false;
        }

        public void ProvideFault(Exception error, System.ServiceModel.Channels.MessageVersion version, ref System.ServiceModel.Channels.Message fault)
        {
        }
    }
}