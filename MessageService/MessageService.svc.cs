using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Xml.Serialization;
using System.Text.RegularExpressions;
using System.Net.Mail;
using System.Configuration;

namespace MessageService
{
    public class MessageService : IMessageService
    {
        private IAddress _address;
        private ISender _sender;
        private IDatabaseConnection _databaseConnection;

        public string address;

        public MessageService()
        {
            _address = new EmailAddress();
            _sender = new EmailSender();
            _databaseConnection = new DatabaseConnectionMSSQL();
        }

        public MessageResponse Send(MessageRequest message)
        {
            address = _address.GetAddress(message);

            if (message.Recipient.LegalForm == LegalForm.Person)
            {
                if (String.IsNullOrWhiteSpace(message.Recipient.LastName) || String.IsNullOrWhiteSpace(message.Recipient.FirstName))
                {
                    _databaseConnection.WriteToDatabase(message, address, "Name or surname is missing", Convert.ToString(ReturnCode.ValidationError));
                    return new MessageResponse { ErrorMessage = "Name or surname is missing", ReturnCode = ReturnCode.ValidationError };
                }
                if (!message.Recipient.Contacts.Any(x => x.ContactType == ContactType.Email))
                {
                    _databaseConnection.WriteToDatabase(message, address, "Email address is missing", Convert.ToString(ReturnCode.ValidationError));
                    return new MessageResponse { ErrorMessage = "Email address is missing", ReturnCode = ReturnCode.ValidationError };
                }
                    
                if (!message.Recipient.Contacts.Any(x => x.ContactType == ContactType.Email && Regex.IsMatch(x.Value, @"\S+@\S+\.\S+")))
                {
                    _databaseConnection.WriteToDatabase(message, address, "Email address is incorrect", Convert.ToString(ReturnCode.ValidationError));
                    return new MessageResponse { ErrorMessage = "Email address is incorrect", ReturnCode = ReturnCode.ValidationError };
                }
                    
            }

            if (message.Recipient.LegalForm == LegalForm.Company)
            {
                if (String.IsNullOrWhiteSpace(message.Recipient.LastName))
                {
                    _databaseConnection.WriteToDatabase(message, address, "Comapany name is missing", Convert.ToString(ReturnCode.ValidationError));
                    return new MessageResponse { ErrorMessage = "Comapany name is missing", ReturnCode = ReturnCode.ValidationError };
                }
                    
                if (!message.Recipient.Contacts.Any(x => x.ContactType == ContactType.OfficeEmail))
                {
                    _databaseConnection.WriteToDatabase(message, address, "Email address is missing", Convert.ToString(ReturnCode.ValidationError));
                    return new MessageResponse { ErrorMessage = "Email address is missing", ReturnCode = ReturnCode.ValidationError };
                }
                    
                if (!message.Recipient.Contacts.Any(x => x.ContactType == ContactType.OfficeEmail && Regex.IsMatch(x.Value, @"\S+@\S+\.\S+")))
                {
                    _databaseConnection.WriteToDatabase(message, address, "Email address is incorrect", Convert.ToString(ReturnCode.ValidationError));
                    return new MessageResponse { ErrorMessage = "Email address is incorrect", ReturnCode = ReturnCode.ValidationError };
                }
                    
            }

            _sender.SendMessage(message, address);

            _databaseConnection.WriteToDatabase(message, address);

            return null;
        }
    }
}