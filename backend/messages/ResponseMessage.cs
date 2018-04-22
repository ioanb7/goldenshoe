using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Messages
{
    public class ResponseMessage
    {
        public ResponseMessage(string message)
        {
            Message = message;
        }

        public string Message { get; set; }
    }
}
