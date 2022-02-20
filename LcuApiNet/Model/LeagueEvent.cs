using LcuApiNet.Core.Events;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LcuApiNet.Model
{
    public class LeagueEvent
    {
        [JsonProperty("eventType"), JsonConverter(typeof(StringEnumConverter))]
        public LeagueInternalEventType InternalEventType { get; set; }

        [JsonProperty("uri")]
        public string Uri { get; set; } = null!;

        [JsonProperty("data")]
        public object? Data { get; set; }

    }
}
