using LcuApiNet.Exceptions;

namespace LcuApiNet.Model
{
    /// <summary>
    /// Contains credentials used to access league client api.
    /// </summary>
    public class LcuCredentials
    {
        /// <summary>
        /// Port of the internal league server.
        /// </summary>
        public int Port;

        /// <summary>
        /// Password that should be provided in authorization header 
        /// in order to execute api methods.
        /// </summary>
        public string Password;

        /// <summary>
        /// Protocol that should be used when creating a request to api.
        /// </summary>
        public string Protocol;
        
        /// <summary>
        /// Parse credentials string into credentials object.
        /// Credentials should match the pattern: `{LeagueClient}:{ProcessName}:{Port}:{Password}:{Protocol}`.
        /// </summary>
        /// <param name="credentials">Credentials string matching the pattern above.</param>
        /// <exception cref="IncorrectCredentialsException"/>
        /// <returns>Credentials object</returns>
        public static LcuCredentials FromString(string credentials)
        {
            Console.WriteLine($"[{credentials}] {DateTime.Now:HH:mm:ss:ffff}");

            string[] splitted = credentials.Split(':');
            if (splitted.Length != 5) {
                throw new IncorrectCredentialsException(credentials);
            }
            
            try {
                return new LcuCredentials(
                    Convert.ToInt32(splitted[2]), splitted[3], splitted[4]);
            } catch (FormatException) {
                throw new IncorrectCredentialsException(credentials);
            } catch (OverflowException) {
                throw new IncorrectCredentialsException(credentials);
            }
        }

        private LcuCredentials(int port, string password, string protocol) =>
            (Port, Password, Protocol) = (port, password, protocol);
    }
}
