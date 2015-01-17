using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MModalClientSPA.ErrorHandler.Exceptions
{
    public class HostException : Exception
    {
        string message;

        public override string Message
        {
            get
            {
                return message;
            }
        }

        public HostException(string message) : base(message)
        {
            this.message = message;
        }

        public override string ToString()
        {
            return message;
        }
    }
}