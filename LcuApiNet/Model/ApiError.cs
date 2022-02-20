using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace LcuApiNet.Model
{
    /// <summary>
    /// Describe message that is sent back on API error.
    /// </summary>
    public class ApiError
    {
        public ErrorCode Code { get; }
        public string Message { get; }

        public ApiError(ErrorCode code, string message) =>
            (Code, Message) = (code, message);
    }
}
