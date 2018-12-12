using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace MessageService
{
    //Validating input message
    public class MessageValidator
    {
        private MessageRequest message { get; set; }

        private string recipientAddress { get; set; }

        public MessageValidator(MessageRequest message, string recipientAddress)
        {
            this.message = message;
            this.recipientAddress = recipientAddress;
        }

        public string ValidateMessage()
        {
            //Case: Recipient is person
            if (message.Recipient.LegalForm == LegalForm.Person)
            {
                //Case: Missing person name or surname
                if (String.IsNullOrWhiteSpace(message.Recipient.LastName) || String.IsNullOrWhiteSpace(message.Recipient.FirstName))
                    return "Name or surname is missing";

                //Case: Missing person email address
                else if (recipientAddress == null)
                    return "Email address is missing";

                //Case: Incorrect person email address
                else if (!Regex.IsMatch(recipientAddress, GlobalConst.EmailValidationPattern))
                    return "Email address is incorrect";
            }

            //Case: Recipient is company
            if (message.Recipient.LegalForm == LegalForm.Company)
            {
                //Case: Missing company name
                if (String.IsNullOrWhiteSpace(message.Recipient.LastName))
                    return "Company name is missing";

                //Case: Missing company email address
                else if (recipientAddress == null)
                    return "Email address is missing";

                //Case: Incorrect company email address
                else if (!Regex.IsMatch(recipientAddress, GlobalConst.EmailValidationPattern))
                    return "Email address is incorrect";

            }

            return "Message validated successfully";
        }



    }
}