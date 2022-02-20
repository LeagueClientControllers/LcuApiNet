using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LcuApiNet.Core.Events
{
    [AttributeUsage(AttributeTargets.Event)]
    public class LeagueEventRepeaterAttribute : Attribute
    {
        public LeagueEventType EventType { get; set; }
        public string CategoryUri { get; set; } 
        public Type DataType { get; set; }
        public string Uri { get; set; }

        public LeagueEventRepeaterAttribute(LeagueEventType eventType, string categoryUri, Type dataType, string uri)
        {
            EventType = eventType;
            CategoryUri = categoryUri;
            DataType = dataType;
            Uri = uri;
        }
    }
}
