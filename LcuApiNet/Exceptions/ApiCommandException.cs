using LcuApiNet.Model;

namespace LcuApiNet.Exceptions
{
    /// <summary>
    /// The exception that is thrown when error occurred when executing API command.
    /// </summary>
    public class ApiCommandException : Exception
    {
        public ApiError Details { get; set; }

        public ApiCommandException(ApiError details): base($"Api command exception. {details.Code}: {details.Message}")
        {
            Details = details;
        }
    }
}
