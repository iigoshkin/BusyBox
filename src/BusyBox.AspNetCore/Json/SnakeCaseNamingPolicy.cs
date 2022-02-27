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

            int countCharUpper = GetCountCharUpper(name);

            int indexWrite = indexStart;

            var arrayPool = ArrayPool<char>.Shared;
            char[] buffer = arrayPool.Rent(name.Length + countCharUpper);

            if (char.IsUpper(name[0]))
                buffer[0] = char.ToLower(name[0]);

            for (int indexRead = indexStart; indexRead < name.Length; indexRead++)
            {
                if (char.IsUpper(name[indexRead]))
                {
                    buffer[indexWrite] = splitChar;
                    buffer[++indexWrite] = char.ToLower(name[indexRead]);
                    indexWrite++;
                }
                else
                {
                    buffer[indexWrite] = name[indexRead];
                    indexWrite++;
                }
            }

            var str = new string(new ReadOnlySpan<char>(buffer, 0, indexWrite));
            arrayPool.Return(buffer, true);

            return str;
        }

        private static int GetCountCharUpper(string str)
        {
            int count = 0;
            for (int index = 0; index < str.Length; index++)
            {
                if (char.IsUpper(str[index]))
                    count++;
            }

            return count;
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
