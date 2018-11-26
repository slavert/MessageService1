using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace MessageService
{
    //Resolving recipient address from input message
    public class RecipientEmailAddressResolver : IRecipientAddressResolver
    {
        IDatabaseConnection DatabaseConnection { get; set; }

        public RecipientEmailAddressResolver(IDatabaseConnection databaseConnection)
        {
            DatabaseConnection = databaseConnection;
        }

        public string GetRecipientAddress(MessageRequest message, ref MessageResponse messageResponse)
        {
            try
            {
                //Checking if input message contains correct email address if not checking value from email address node
                if (message.Recipient.LegalForm == LegalForm.Person)
                {
                    if (message.Recipient.Contacts.Any(x => x.ContactType == ContactType.Email && Regex.IsMatch(x.Value, GlobalConst.EmailValidationPattern)))
                        return message.Recipient.Contacts.FirstOrDefault(x => x.ContactType == ContactType.Email && Regex.IsMatch(x.Value, GlobalConst.EmailValidationPattern)).Value;
                    else if (message.Recipient.Contacts.Any(x => x.ContactType == ContactType.Email))
                        return message.Recipient.Contacts.FirstOrDefault(x => x.ContactType == ContactType.Email).Value;
                    else return null;
                }

                else
                {
                    if (message.Recipient.Contacts.Any(x => x.ContactType == ContactType.OfficeEmail && Regex.IsMatch(x.Value, GlobalConst.EmailValidationPattern)))
                        return message.Recipient.Contacts.FirstOrDefault(x => x.ContactType == ContactType.Email && Regex.IsMatch(x.Value, GlobalConst.EmailValidationPattern)).Value;
                    else if (message.Recipient.Contacts.Any(x => x.ContactType == ContactType.OfficeEmail))
                        return message.Recipient.Contacts.FirstOrDefault(x => x.ContactType == ContactType.Email).Value;
                    else return null;
                }
            }
            catch (Exception error)
            {
                //Logging error
                messageResponse = new MessageResponse()
                {
                    ErrorMessage = error.Message,
                    ReturnCode = ReturnCode.InternalError
                };
                
                DatabaseConnection.WriteToDatabase(null, null, ref messageResponse);
                return null;
            }
            
        }

    }

    public interface IRecipientAddressResolver
    {
        string GetRecipientAddress(MessageRequest message, ref MessageResponse messageResponse);
    }

    
}
