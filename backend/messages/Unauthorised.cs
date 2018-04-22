using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Messages
{
    public class Unauthorised : ResponseMessage
    {
        public Unauthorised() : base("Unauthorised")
        {
        }
    }
}
