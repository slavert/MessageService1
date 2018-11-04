using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace MessageService
{
    public class EmailAddress : IAddress
    {
        public string GetAddress(MessageRequest message)
        {
            if (message.Recipient.LegalForm == LegalForm.Person)
            return message.Recipient.Contacts.Single(x => x.ContactType == ContactType.Email && Regex.IsMatch(x.Value, @"\S+@\S+\.\S+")).Value;
            else
            return message.Recipient.Contacts.Single(x => x.ContactType == ContactType.OfficeEmail && Regex.IsMatch(x.Value, @"\S+@\S+\.\S+")).Value;
        }
    }

    public interface IAddress
    {
        string GetAddress(MessageRequest message);
    }
}