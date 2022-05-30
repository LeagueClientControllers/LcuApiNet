using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LcuApiNet.Core.Events;

namespace LcuApiNet.EventHandlers
{
    public delegate void LeagueEventHandler<in T>(object sender, LeagueInternalEventType eventType, T args);
    
}
