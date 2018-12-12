using Ninject;
using System;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;

namespace MessageService
{ 
    //Input message internal error handler
    public class InternalErrorHandler : IErrorHandler
    {

        public bool HandleError(Exception error)
        {
            IKernel kernel = new StandardKernel();
            kernel.Load(Assembly.GetExecutingAssembly());

            var DatabaseConnection = kernel.Get<IDatabaseConnection>();

            //Writin error to database
            DatabaseConnection.WriteToDatabase(null, null, new MessageResponse( ReturnCode.InternalError, error.Message ));
            
            return true;
        }

        public void ProvideFault(Exception error, System.ServiceModel.Channels.MessageVersion version, ref System.ServiceModel.Channels.Message fault)
        {
            MessageResponse messageResponseError = new MessageResponse(error);

            FaultException<MessageResponse> faultException = new FaultException<MessageResponse>(messageResponseError);

            MessageFault messageFault = faultException.CreateMessageFault();

            fault = Message.CreateMessage(version, messageFault, null);
        }
    }
}