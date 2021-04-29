namespace BusyBox.AspNetCore.Jwt
{
	public class JwtResponse
	{
		public string Token { get; }

		public long ExpiresAt { get; }

		public JwtResponse()
		{
		}

		public JwtResponse(string token, long expiresAt)
		{
			Token = token;
			ExpiresAt = expiresAt;
		}
	}
}
