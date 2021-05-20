using System;
using System.IO;
using System.Security.Cryptography;
using BusyBox.AspNetCore.Extensions;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace BusyBox.AspNetCore.Jwt.Security
{
    public class AsymmetricSigning : ISigning, IDisposable
    {
        private readonly RSA _signing;
        private readonly IOptionsMonitor<JwtSecurityOptions> _options;

        private bool _disposed;

        public AsymmetricSigning(IOptionsMonitor<JwtSecurityOptions> options)
        {
            _options = options;
            _signing = RSA.Create();
        }

        public SecurityKey GetSecurityKey()
        {
            CheckObjectDispose();
            JwtSecurityOptions options = _options.CurrentValue;
            if (options == null)
                throw new ArgumentException("Missing config section for Jwt");

            if (!File.Exists(options.PathPublicKey))
                throw new FileNotFoundException(options.PathPublicKey);

            string pemContents = File.ReadAllText(options.PathPublicKey);
            ReadOnlySpan<byte> der = pemContents.EncodePemContent();
            _signing.ImportSubjectPublicKeyInfo(der, out _);

            return new RsaSecurityKey(_signing);
        }

        public SigningCredentials CreateSigning()
        {
            CheckObjectDispose();
            JwtSecurityOptions options = _options.CurrentValue;
            if (options == null)
                throw new ArgumentException("Missing config section for Jwt");

            if (!File.Exists(options.PathPrivateKey))
                throw new FileNotFoundException(options.PathPrivateKey);

            string pemContents = File.ReadAllText(options.PathPrivateKey);

            ReadOnlySpan<byte> der = pemContents.EncodePemContent();
            _signing.ImportRSAPrivateKey(der, out _);

            return new SigningCredentials(new RsaSecurityKey(_signing), SecurityAlgorithms.RsaSha256)
            {
                CryptoProviderFactory = new CryptoProviderFactory { CacheSignatureProviders = false }
            };
        }

        private void CheckObjectDispose()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(_signing));
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            _signing.Dispose();
            _disposed = true;
        }
    }
}
