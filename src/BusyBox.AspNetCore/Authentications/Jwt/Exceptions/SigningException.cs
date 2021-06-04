using System;

namespace BusyBox.AspNetCore.Authentications.Jwt.Exceptions
{
    public class SigningException : Exception
    {
        public SigningException(string message): base(message)
        {
        }

        public SigningException(string message, Exception? innerException): base(message, innerException)
        {
        }
    }
}
