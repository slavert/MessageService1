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
        private IRecipientAddressResolver _recipientAddressResolver;
        private ISender _sender;
        private IDatabaseConnection _databaseConnection;

        public string RecipientAddress;
        public MessageRequest Message;

        public MessageService()
        {
            _recipientAddressResolver = new RecipientEmailAddressResolver();
            _sender = new EmailSender();
            _databaseConnection = new DatabaseConnectionMSSQL();
        }

        public MessageResponse Send(MessageRequest message)
        {
            MessageResponse messageResponse = new MessageResponse(); // will remain null if validation will be successful
            Message = message;
            RecipientAddress = _recipientAddressResolver.GetRecipientAddress(message);

            //Response to be sent using interface
            messageResponse = MessageValidation();

            //Send response using interface if message validation was unsuccessful
            if (messageResponse != null)
            {
                //Log actions to database
                _databaseConnection.WriteToDatabase(Message, RecipientAddress, messageResponse.ErrorMessage, Convert.ToString(messageResponse.ReturnCode));

                return messageResponse;
            }
            else
            {
                //Log actions to database
                _databaseConnection.WriteToDatabase(Message, RecipientAddress, null, null);

                //Send requested message to recipient
                _sender.SendMessage(Message, RecipientAddress);

                return null;
            }
            
        }

        private MessageResponse MessageValidation()
        {
            //Case: Recipient is person
            if (Message.Recipient.LegalForm == LegalForm.Person)
            {
                //Case: Missing person name or surname
                if (String.IsNullOrWhiteSpace(Message.Recipient.LastName) || String.IsNullOrWhiteSpace(Message.Recipient.FirstName))
                    return new MessageResponse { ErrorMessage = "Name or surname is missing", ReturnCode = ReturnCode.ValidationError };

                //Case: Missing person email address
                else if (!Message.Recipient.Contacts.Any(x => x.ContactType == ContactType.Email))
                    return new MessageResponse { ErrorMessage = "Email address is missing", ReturnCode = ReturnCode.ValidationError };

                //Case: Incorrect person email address
                else if (!Message.Recipient.Contacts.Any(x => x.ContactType == ContactType.Email && Regex.IsMatch(x.Value, GlobalConst.EmailValidationPattern)))
                    return new MessageResponse { ErrorMessage = "Email address is incorrect", ReturnCode = ReturnCode.ValidationError };
            }

            //Case: Recipient is company
            if (Message.Recipient.LegalForm == LegalForm.Company)
            {
                //Case: Missing comapny name
                if (String.IsNullOrWhiteSpace(Message.Recipient.LastName))
                    return new MessageResponse { ErrorMessage = "Comapany name is missing", ReturnCode = ReturnCode.ValidationError };

                //Case: Missing company email address
                else if (!Message.Recipient.Contacts.Any(x => x.ContactType == ContactType.OfficeEmail))
                    return new MessageResponse { ErrorMessage = "Email address is missing", ReturnCode = ReturnCode.ValidationError };

                //Case: Incorrect company email address
                else if (!Message.Recipient.Contacts.Any(x => x.ContactType == ContactType.OfficeEmail && Regex.IsMatch(x.Value, GlobalConst.EmailValidationPattern)))
                    return new MessageResponse { ErrorMessage = "Email address is incorrect", ReturnCode = ReturnCode.ValidationError };

            }
            return null;
        }

    }
}