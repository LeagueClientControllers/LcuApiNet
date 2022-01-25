namespace LcuApiNet.Exceptions
{
    /// <summary>
    /// The exception that is thrown when client is not ready 
    /// to accept requests but attempted to run one.
    /// </summary>
    public class ClientNotReadyException : Exception
    {
        public ClientNotReadyException() : base("Attempted to run a request to the client when it's not ready") { }
    }
}
