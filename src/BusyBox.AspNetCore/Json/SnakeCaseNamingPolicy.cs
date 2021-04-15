using System;
using System.Buffers;
using System.Text.Json;

namespace BusyBox.AspNetCore.Json
{
    /// <summary>
    /// Implements the SnakeCase naming policy for properties JSON
    /// <example>user_name</example>
    /// </summary>
    public class SnakeCaseNamingPolicy : JsonNamingPolicy
    {
        /// <summary>
        /// Get instance SnakeCaseNamingPolicy
        /// </summary>
        public static SnakeCaseNamingPolicy Instance => new SnakeCaseNamingPolicy();

        public override string ConvertName(string name)
        {
            const int indexStart = 1;
            const char splitChar = '_';

            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            ReadOnlySpan<char> charset = name.ToCharArray();
            int countCharUpper = GetCountCharUpper(charset);

            int indexWrite = indexStart;

            var arrayPool = ArrayPool<char>.Shared;
            char[] buffer = arrayPool.Rent(charset.Length + countCharUpper);

            if (char.IsUpper(charset[0]))
                buffer[0] = char.ToLower(charset[0]);

            for (int indexRead = indexStart; indexRead < charset.Length; indexRead++)
            {
                if (char.IsUpper(charset[indexRead]))
                {
                    buffer[indexWrite] = splitChar;
                    buffer[++indexWrite] = char.ToLower(charset[indexRead]);
                    indexWrite++;
                }
                else
                {
                    buffer[indexWrite] = charset[indexRead];
                    indexWrite++;
                }
            }

            var str = new string(new ReadOnlySpan<char>(buffer, 0, indexWrite));
            arrayPool.Return(buffer, true);

            return str;
        }

        private static int GetCountCharUpper(ReadOnlySpan<char> span)
        {
            int count = 0;
            for (int index = 0; index < span.Length; index++)
            {
                if (char.IsUpper(span[index]))
                    count++;
            }

            return count;
        }
    }
}
