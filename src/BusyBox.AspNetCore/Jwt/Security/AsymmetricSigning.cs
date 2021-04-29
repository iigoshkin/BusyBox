using System;
using System.IO;
using System.Security;
using System.Security.Cryptography;
using BusyBox.AspNetCore.Extensions;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace BusyBox.AspNetCore.Jwt.Security
{
    public class AsymmetricSigning : ISigning, IDisposable
    {
        private readonly RSA _signing;
        private readonly string _pathPublicKey;
        private readonly string _pathPrivateKey;

        private bool _disposed;

        public AsymmetricSigning(IOptions<SigningSetting> options)
        {
            SigningSetting setting = options.Value;
            if (string.IsNullOrEmpty(setting.PathPrivateKey))
                throw new SecurityException("Path the private key cannot be empty");

            if (string.IsNullOrEmpty(setting.PathPublicKey))
                throw new SecurityException("Path the public key cannot be empty");

            _signing = RSA.Create();
            _pathPrivateKey = setting.PathPrivateKey;
            _pathPublicKey = setting.PathPublicKey;
        }

        public SecurityKey GetSecurityKey()
        {
            CheckObjectDispose();
            if (!File.Exists(_pathPublicKey))
                throw new FileNotFoundException(_pathPublicKey);

            string pemContents = File.ReadAllText(_pathPublicKey);
            ReadOnlySpan<byte> der = pemContents.EncodePemContent();
            _signing.ImportSubjectPublicKeyInfo(der, out _);

            return new RsaSecurityKey(_signing);
        }

        public SigningCredentials CreateSigning()
        {
            CheckObjectDispose();
            if (!File.Exists(_pathPrivateKey))
                throw new FileNotFoundException(_pathPrivateKey);

            string pemContents = File.ReadAllText(_pathPrivateKey);

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
