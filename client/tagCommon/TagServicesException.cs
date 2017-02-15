using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TagCommon
{
    public class TagServicesException : Exception 
    {
        public TagServicesException()
        {
        }

        public TagServicesException(string message)
            : base(message)
        {
        }

        public TagServicesException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}

