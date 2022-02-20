using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LcuApiNet.EventHandlers
{
    public delegate void LeagueEventHandler<in T>(object sender, T args);
}
