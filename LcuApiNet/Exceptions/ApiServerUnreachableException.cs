namespace LcuApiNet.Exceptions
{
    /// <summary>
    /// The exception that is thrown when league client API server connection refused.
    /// </summary>
    public class ApiServerUnreachableException : Exception
    {
        public ApiServerUnreachableException() : base("League client API server refused incoming connection") { }
    }
}
