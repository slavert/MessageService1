using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace MessageService
{
    public static class GlobalConst
    {
        public const string EmailValidationPattern = @"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$";
    }

    [ServiceBehavior]
    public class MessageService : IMessageService
    {
        private ISender sender;

        public MessageService()
        {
            sender = new EmailSender(new DatabaseConnectionMSSQL());
        }

        public MessageResponse Send(MessageRequest message)
        {

            MessageValidator messageValidator = new MessageValidator(message, new RecipientEmailAddressResolver(), new DatabaseConnectionMSSQL());

            //Send response using interface if message validation was successful
            if (messageValidator.MessageValidationPassed)
            {
                //Send requested message to recipient
                sender.SendMessage(message, messageValidator.RecipientAddress);
                //Send null response using interface
                return null;
            }
            else
            {
                //Send response using interface
                return messageValidator.messageResponse;
            }
            
        }


    }
}