using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Messages
{
    public class ErrorOccured: ResponseMessage
    {
        public ErrorOccured(string message) : base(message)
        {
        }
    }
}
