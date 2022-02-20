using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LcuApiNet.Exceptions
{
    public class EventNotRegisteredException : Exception
    {
        public EventNotRegisteredException(string message) : base($"{message}") { } 
    }
}
