﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace MessageService
{
    public class MessageValidator
    {
        IRecipientAddressResolver RecipientAddressResolver { get; set; }
        IDatabaseConnection DatabaseConnection { get; set; }
        
        MessageRequest Message { get; set; }

        public MessageResponse messageResponse { get; set; }
        public string RecipientAddress { get; set; }

        public bool MessageValidationPassed { get; set; }

        public MessageValidator(MessageRequest message, IRecipientAddressResolver recipientAddressResolver, IDatabaseConnection databaseConnection)
        {
            Message = message;
            RecipientAddress = recipientAddressResolver.GetRecipientAddress(message);
            messageResponse = ValidateMessage();

            //Log actions to database if message did not pass validation
            if (!MessageValidationPassed)
                DatabaseConnection.WriteToDatabase(Message, RecipientAddress, messageResponse.ErrorMessage, Convert.ToString(messageResponse.ReturnCode));
        }

        public MessageResponse ValidateMessage()
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

            MessageValidationPassed = true;

            return null;
        }



    }
}