namespace BusyBox.AspNetCore.Authentications.Jwt
{
	/// <summary>
	/// Token creation result
	/// </summary>
    public class JwtResponse
	{
        /// <summary>
        /// Token
        /// </summary>
        public string Token { get; }

        /// <summary>
        /// Token expiration time
        /// </summary>
		public long ExpiresAt { get; }

		public JwtResponse(string token)
        {
            Token = token;
        }

		public JwtResponse(string token, long expiresAt) : this(token)
		{
            ExpiresAt = expiresAt;
		}
	}
}
