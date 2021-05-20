namespace BusyBox.AspNetCore.Jwt
{
    public class JwtSecurityOptions
    {
        public string PathPrivateKey { get; set; }

        public string PathPublicKey { get; set; }

        public string SecretKey { get; set; }

        public string Issuer { get; set; }

        public string Expires { get; set; }

        public string Audience { get; set; }
    }
}
