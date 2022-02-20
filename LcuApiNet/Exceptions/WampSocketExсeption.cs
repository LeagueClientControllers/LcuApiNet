using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LcuApiNet.Exceptions
{
    public class WampSocketExсeption : Exception
    {
        public WampSocketExсeption(string? message) : base(message) { }
    }
}
