using System;
using System.Text;

namespace TurnerSoftware.CascadingStyles
{
	public static class TokenizationHelper
	{
		public static bool IsHexDigit(char value) => char.IsDigit(value) ||
			(value >= 'a' && value <= 'f') || (value >= 'A' && value <= 'F');

		/// <summary>
		/// <a href="https://drafts.csswg.org/css-syntax/#consume-escaped-code-point">4.3.7. Consume an escaped code point</a>
		/// </summary>
		/// <remarks>
		/// This completes the core work of the tokenizer and turns hexidecimal values to their real character.
		/// </remarks>
		/// <param name="value"></param>
		/// <returns></returns>
		public static string UnescapeValue(ReadOnlySpan<char> value)
		{
			if (value.Length == 0)
			{
				return string.Empty;
			}

			// Find the first U+005C REVERSE SOLIDUS (\)
			var index = value.IndexOf('\\');
			if (index != -1)
			{
				var builder = new StringBuilder();
				var previousIndex = 0;
				while (index < value.Length)
				{
					// The current index is an escape value and the next one exists
					if (index != -1 && index + 1 < value.Length)
					{
						var current = value[++index];
						if (IsHexDigit(current))
						{
							// Consume as many hex digits as possible, but no more than 5.
							// Note that this means 1-6 hex digits have been consumed in total.
							var hexStartIndex = index;
							var hexDigitCount = 0;
							while (hexDigitCount++ < 5)
							{
								current = value[index + hexDigitCount];
								if (IsHexDigit(current))
								{

								}
								break;
							}
							index += hexDigitCount;

							// If the next input code point is whitespace, consume it as well.
							if (current == ' ')
							{
								index++;
							}

							// Interpret the hex digits as a hexadecimal number.
							var hexSpan = value[hexStartIndex..(hexStartIndex + hexDigitCount)];
							var codePoint = int.Parse(hexSpan, System.Globalization.NumberStyles.HexNumber);

							// If this number is zero, or is for a surrogate, or is greater than the maximum allowed code point,
							// return U+FFFD REPLACEMENT CHARACTER (�).
							if (codePoint == 0 || char.IsSurrogate((char)codePoint) || codePoint > 0x10FFFF)
							{
								builder.Append((char)0xFFFD);
							}
							// Otherwise, return the code point with that value.
							else
							{
								builder.Append(char.ConvertFromUtf32(codePoint));
							}
							continue;
						}

						builder.Append(current);
					}
					// If there are no more values to escape, append the rest of the span
					else
					{
						builder.Append(value[previousIndex..]);
						break;
					}

					// Track our new position and find the next U+005C REVERSE SOLIDUS (\)
					previousIndex = index + 1;
					index = value[index..].IndexOf('\\');
				}

				// Build the unescaped value to a string
				return builder.ToString();
			}

			// Don't have one? Turn the span into a string
			return value.ToString();
		}

		public static string UnescapeValue(this CssToken token) => UnescapeValue(token.RawValue);

		public static string UnescapeSecondaryValue(this CssToken token) => UnescapeValue(token.RawSecondaryValue);

		public static decimal GetNumber(this CssToken token) => token.Type switch
		{
			CssTokenType.Number or CssTokenType.Dimension => decimal.Parse(token.RawValue, System.Globalization.NumberStyles.Number),
			_ => throw new InvalidOperationException("Token is not of a valid type to decode a number from"),
		};
	}
}
