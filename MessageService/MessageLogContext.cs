using System;
using System.Data.Entity;
using System.Linq;

namespace MessageService
{

    public class MessageLogContext : DbContext
    {

        public MessageLogContext()
            : base("name=MessageLogContext")
        {
        }

        public DbSet<MessageLog> MessageLogDbSet { get; set; }

    }

}