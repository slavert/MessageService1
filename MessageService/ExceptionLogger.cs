using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MessageService
{
    public class ExceptionLogger
    {
        public static MessageResponse Logs;

        public static bool LogsNonEmpty;

        public static void Log(ReturnCode returnCode, string errorMessage)
        {
            Logs = new MessageResponse(){ReturnCode = returnCode, ErrorMessage = errorMessage};
            LogsNonEmpty = false;
        }
    }
}