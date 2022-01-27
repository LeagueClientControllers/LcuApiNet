using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace LcuApiNet.Model
{
    /// <summary>
    /// Describe message that is sent back on API error.
    /// </summary>
    public class ApiError
    {
        [JsonProperty("errorCode")]
        [JsonConverter(typeof(StringEnumConverter))]
        public ErrorCode Code { get; set; }
        
        [JsonProperty("httpStatus")]
        public int HttpStatus { get; set; }

        [JsonProperty("implementationDetails")]
        public Dictionary<dynamic, dynamic> ImplementationDetails { get; set; } = null!;

        [JsonProperty("message")]
        public string Message { get; set; } = null!;
    }
}
