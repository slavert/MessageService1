﻿using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text.RegularExpressions;

namespace MessageService
{
    [ServiceContract]
    public interface IMessageService
    {
        [OperationContract]
        MessageResponse Send(MessageRequest message);
    }

    [DataContract]
    public class MessageRequest
    {
        [DataMember(IsRequired = true, EmitDefaultValue = false)]
        public string Subject { get; set; }
        [DataMember(IsRequired = true, EmitDefaultValue = false)]
        public string Message { get; set; }
        [DataMember(IsRequired = true, EmitDefaultValue = false)]
        public Recipient Recipient { get; set; }
    }

    [DataContract]
    public class Recipient
    {
        [DataMember]
        public string FirstName { get; set; }
        [DataMember]
        public string LastName { get; set; }
        [DataMember(IsRequired = true)]
        public LegalForm LegalForm { get; set; }
        [DataMember(IsRequired = false)]
        public Contact[] Contacts { get; set; }
    }

    [DataContract]
    public class Contact
    {
        [DataMember]
        public ContactType ContactType { get; set; }
        [DataMember]
        public string Value { get; set; }
    }

    [DataContract]
    public class MessageResponse
    {
        [DataMember]
        public ReturnCode ReturnCode { get; set; }
        [DataMember]
        public string ErrorMessage { get; set; }
    }

    [DataContract]
    public enum ReturnCode
    {
        [EnumMember]
        Success,
        [EnumMember]
        ValidationError,
        [EnumMember]
        InternalError
    }

    [DataContract]
    public enum LegalForm
    {
        [EnumMember]
        Person,
        [EnumMember]
        Company
    }

    [DataContract]
    public enum ContactType
    {
        [EnumMember]
        Mobile,
        [EnumMember]
        Fax,
        [EnumMember]
        Email,
        [EnumMember]
        OfficePhone,
        [EnumMember]
        OfficeFax,
        [EnumMember]
        OfficeEmail
    }
}