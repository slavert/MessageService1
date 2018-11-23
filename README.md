# MessageService1
This project purpose is to create WCF SOAP service for handling email message requests.

We suppose our company wants to send email messages to its clients.
Input message definition can be found in IMessageService.cs file.

This input message is subject to validation:

1. If LegalForm is Person then 
a) FirstName and LastName are required.
b) Contacts must contain ContactType=Email  
c) ContactType=Email must contain valid email address
2. If LegalForm is Company then
a) LastName is required.
b) Contacts must contain ContactType=OfficeEmail  
c) ContactType=OfficeEmail must contain valid email address

If validation fails, service sends output message containing ReturnCode and Error description.
In other case email message is sent to appropriate address. (Email for persons, OfficeEmail for companies)
For this purpose SMTP Server is used. Settings can be found in Web.config file.

The service logs errors to database if encounters them. While connecting it uses Entity Framework with First Code approach.

In order to resolve class dependencies Ninject container is used.
