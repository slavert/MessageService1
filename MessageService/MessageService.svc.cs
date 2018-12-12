using System;
using System.Linq;
using System.Text.RegularExpressions;
using Ninject;
using Ninject.Modules;
using System.Reflection;

namespace MessageService
{
    public static class GlobalConst
    {
        public const string EmailValidationPattern = @"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$";
    }

    [ServiceBehavior]
    public class MessageService : IMessageService
    {

        private string recipientAddress { get; set; }

        public MessageResponse Send(MessageRequest message)
        {
            //Setting dependency injection container
            IKernel kernel = new StandardKernel();
            kernel.Load(Assembly.GetExecutingAssembly());

            var databaseConnection = kernel.Get<IDatabaseConnection>();
            var recipientAddressResolver = kernel.Get<IRecipientAddressResolver>();

            recipientAddress = recipientAddressResolver.GetRecipientAddress(message);

            MessageValidator messageValidator = new MessageValidator(message, recipientAddress);

            string validationResult = messageValidator.ValidateMessage();

            //Send response using interface if input message validation was successful
            if (validationResult == "Message validated successfully")
            {
                var sender = kernel.Get<ISender>();
                //Send requested message to recipient
                sender.SendMessage(message, recipientAddress);
            }

            //Write exceptions to database
            if (!ExceptionLogger.LogsNonEmpty)
                databaseConnection.WriteToDatabase(message, recipientAddress, null);
            else
                databaseConnection.WriteToDatabase(message, recipientAddress, ExceptionLogger.Logs);

            //Return response
            if (!ExceptionLogger.LogsNonEmpty && validationResult == "Message validated successfully")
                return new MessageResponse() { ReturnCode = ReturnCode.Success };
            else if (!ExceptionLogger.LogsNonEmpty)
                return new MessageResponse() { ReturnCode = ReturnCode.ValidationError, ErrorMessage = validationResult };
            else
                return ExceptionLogger.Logs;
            
        }


    }

    //Dependency injection container bindings
    public class Bindings : NinjectModule
    {
        public override void Load()
        {
            Bind<IDatabaseConnection>().To<DatabaseConnectionMSSQL>();
            Bind<IRecipientAddressResolver>().To<RecipientEmailAddressResolver>();
            Bind<ISender>().To<EmailSender>();
        }
    }
}