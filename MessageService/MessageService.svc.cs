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

        public MessageResponse Send(MessageRequest message)
        {
            //Setting dependency injection container
            IKernel kernel = new StandardKernel();
            kernel.Load(Assembly.GetExecutingAssembly());

            var DatabaseConnection = kernel.Get<IDatabaseConnection>();
            var RecipientAddressResolver = kernel.Get<IRecipientAddressResolver>();

            MessageResponse messageResponse = null;
            MessageValidator messageValidator = new MessageValidator(message, ref messageResponse, RecipientAddressResolver, DatabaseConnection);

            //Send response using interface if input message validation was successful
            if (messageValidator.MessageValidationPassed)
            {
                ISender sender = new EmailSender(DatabaseConnection);
                //Send requested message to recipient
                sender.SendMessage(message, messageValidator.RecipientAddress, ref messageResponse);
            }
            if (messageResponse == null)
                messageResponse = new MessageResponse() { ReturnCode = ReturnCode.Success };
            return messageResponse;
            
        }


    }

    //Dependency injection container bindings
    public class Bindings : NinjectModule
    {
        public override void Load()
        {
            Bind<IDatabaseConnection>().To<DatabaseConnectionMSSQL>();
            Bind<IRecipientAddressResolver>().To<RecipientEmailAddressResolver>();
        }
    }
}