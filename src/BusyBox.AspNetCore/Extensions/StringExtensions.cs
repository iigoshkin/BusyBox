using System;

namespace BusyBox.AspNetCore.Extensions
{
    public static class StringExtensions
    {
        /// <summary>
        /// Encoding content from PEM file
        /// </summary>
        /// <param name="pemContents">Content PEM file</param>
        /// <returns></returns>
        public static ReadOnlySpan<byte> EncodePemContent(this string pemContents)
        {
            string header = pemContents.IndexOf("PRIVATE", StringComparison.OrdinalIgnoreCase) > -1
                ? "RSA PRIVATE"
                : "PUBLIC";

            string rsaBeginHeaderKey = $"-----BEGIN {header} KEY-----";
            string rsaFooterHeaderKey = $"-----END {header} KEY-----";

            int endIdx = pemContents.IndexOf(
                rsaFooterHeaderKey,
                rsaBeginHeaderKey.Length,
                StringComparison.Ordinal);

            string base64 = pemContents.Substring(
                rsaBeginHeaderKey.Length,
                endIdx - rsaBeginHeaderKey.Length);

            return Convert.FromBase64String(base64);
        }
    }
}
