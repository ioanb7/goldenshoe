using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Messages
{
    public class ResponseMessageWithStatus
    {
        public ResponseMessageWithStatus(string message, bool status)
        {
            Message = message;
            Status = status;
        }

        public string Message { get; set; }
        public bool Status { get; set; }
    }
}