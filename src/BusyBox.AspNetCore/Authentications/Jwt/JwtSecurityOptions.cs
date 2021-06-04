namespace BusyBox.AspNetCore.Authentications.Jwt
{
    public class JwtSecurityOptions
    {
        public string? PathPrivateKey { get; set; }

        public string? PathPublicKey { get; set; }

        public string? SecretKey { get; set; }

        public string Issuer { get; set; } = "iss"!;

        public string Expires { get; set; } = "00:30:00"!;

        public string Audience { get; set; } = "au"!;
    }
}
