using System;
using System.Text.Json;

namespace BusyBox.AspNetCore.Json
{
	public class SnakeCaseNamingPolicy : JsonNamingPolicy
	{
		public static SnakeCaseNamingPolicy Instance { get; } = new SnakeCaseNamingPolicy();

		public override string ConvertName(string name)
		{
			const int indexStart = 1;
			const char splitChar = '_';

			if (string.IsNullOrEmpty(name))
				throw new ArgumentNullException(nameof(name));

			ReadOnlySpan<char> charset = name.ToCharArray();
			int countCharUpper = GetCountCharUpper(charset);
			if (countCharUpper == 0)
				return name;

			int sizeBuffer = charset.Length + countCharUpper;
			int indexWrite = indexStart;

			Span<char> buffer = sizeBuffer < 255 ? stackalloc char[sizeBuffer] : new char[sizeBuffer];

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

			return new string(buffer[..indexWrite]);
		}

		private static int GetCountCharUpper(ReadOnlySpan<char> charset)
		{
			int count = 1;
			for (int index = 1; index < charset.Length; index++)
				if (char.IsUpper(charset[index]))
					count++;

			return count;
		}
	}
}
