namespace LcuApiNet.Exceptions
{
    /// <summary>
    /// The exception that is thrown when client api command returned wrong response.
    /// </summary>
    public class WrongResponseException : Exception
    {
        public WrongResponseException(string message) : base(message) { }
    }
}
