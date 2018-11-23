using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;

namespace MessageService
{
    //Entity Framework model
    public class MessageLog
    {
        [Key]
        public int ID { get; set; }
        public string EmailAddress { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }
        public string ReturnCode { get; set; }
        public string ErrorMessage { get; set; }
        public DateTime CreationDate { get; set; }

    }
}
