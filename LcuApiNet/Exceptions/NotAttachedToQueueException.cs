namespace LcuApiNet.Exceptions
{
    /// <summary>
    /// The exception that is thrown when request require user to be attached to the matchmaking queue but he isn't.
    /// </summary>
    public class NotAttachedToQueueException : Exception
    {
        public NotAttachedToQueueException() : base("User is not attached to a matchmaking queue") { }
    }
}
