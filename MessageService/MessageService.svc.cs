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

            MessageValidator messageValidator = new MessageValidator(message, RecipientAddressResolver, DatabaseConnection);

            //Send response using interface if input message validation was successful
            if (messageValidator.MessageValidationPassed)
            {
                ISender sender = new EmailSender(DatabaseConnection);
                MessageResponse SenderMessageResponse = null;
                //Send requested message to recipient
                sender.SendMessage(message, messageValidator.RecipientAddress, ref SenderMessageResponse);

                return SenderMessageResponse;
            }
            else
                return messageValidator.messageResponse;
            
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