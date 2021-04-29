namespace BusyBox.AspNetCore.Jwt.Security
{
    public class SigningSetting
    {
        public string? PathPublicKey { get; set; }
        public string? PathPrivateKey { get; set; }

        public string? SecretKey { get; set; }
    }
}
