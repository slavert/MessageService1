# MessageService1
This project purpose is to create WCF SOAP service for handling email message requests.

We suppose our company wants to send email messages to its clients.
Input message and output definitions can be found in MessageService/IMessageService.cs file (MessageRequest with associated classes and MessageResponse respectively).

This input message is subject to validation:

1. If LegalForm is Person then 
a) FirstName and LastName are required
b) Contacts must contain ContactType=Email  
c) ContactType=Email must contain valid email address
2. If LegalForm is Company then
a) LastName is required
b) Contacts must contain ContactType=OfficeEmail  
c) ContactType=OfficeEmail must contain valid email address

If validation fails and there are no other errors service sends output message with ReturnCode=ValidationError.
In case of other error the message contains ReturnCode=InternalError.

Email message is sent to appropriate address. (Email for persons, OfficeEmail for companies)
For this purpose SMTP Server is used. Settings can be found in Web.config file.
The service logs errors to database if encounters them (settings also can be found Web.config file). While connecting it uses Entity Framework with First Code approach.

In order to resolve class dependencies Ninject container is used.

Sample input message is located in examples/ValidMessage.xml.

Service is hosted by IIS.
Default header:
POST http://localhost:8733/MessageService.svc HTTP/1.1
Accept-Encoding: gzip,deflate
Content-Type: text/xml;charset=UTF-8
SOAPAction: "http://tempuri.org/IMessageService/Send"