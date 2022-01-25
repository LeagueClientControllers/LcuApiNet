namespace LcuApiNet.Exceptions
{
    /// <summary>
    /// The exception that is thrown when parsing a string 
    /// with credentials that doesn't match specific pattern
    /// </summary>
    public class IncorrectCredentialsException : Exception
    {
        public IncorrectCredentialsException(string credentials): 
            base($"Attempted to parse credentials string [{credentials}] that doesn't match credentials pattern") { }
    }
}
