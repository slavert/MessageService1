using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MessageService
{
    public class ErrorLogger
    {
        public static MessageResponse Logs { set; get; }

        public static bool LogsNonEmpty { set; get; }

        public static void Log(ReturnCode returnCode, string errorMessage)
        {
            Logs = new MessageResponse(returnCode, errorMessage);
            LogsNonEmpty = false;
        }
    }

}