using System;
using System.Text;

namespace TurnerSoftware.CascadingStyles
{
	public ref struct CssReader
	{
		public const char EndOfFile = char.MaxValue;

		private readonly ReadOnlySpan<char> InputStream;
		private int RegionStartIndex;
		private int IndexOffset;

		public CssReader(ReadOnlySpan<char> value)
		{
			InputStream = value;
			RegionStartIndex = 0;
			IndexOffset = 0;
		}

		private int CurrentIndex => RegionStartIndex + IndexOffset;

		private char Current
		{
			get
			{
				if (CurrentIndex < InputStream.Length)
				{
					return InputStream[CurrentIndex];
				}

				return EndOfFile;
			}
		}

		private char Peek(int distance = 1)
		{
			var nextIndex = CurrentIndex + distance;
			if (nextIndex < InputStream.Length)
			{
				return InputStream[nextIndex];
			}

			return EndOfFile;
		}

		/// <summary>
		/// Returns the region starting from <see cref="RegionStartIndex"/> to, but not including, <see cref="CurrentIndex"/>.
		/// </summary>
		/// <returns></returns>
		private ReadOnlySpan<char> GetRegion()
		{
			return InputStream[RegionStartIndex..CurrentIndex];
		}

		/// <summary>
		/// Finalizes the current region and starts a new region.
		/// Region is from <see cref="RegionStartIndex"/> to, but not including, <see cref="CurrentIndex"/>.
		/// </summary>
		/// <returns></returns>
		private ReadOnlySpan<char> ConsumeRegion()
		{
			var result = GetRegion();
			StartNewCaptureRegion();
			return result;
		}

		/// <summary>
		/// Finalizes the current region to a <see cref="CssToken"/> and starts a new region.
		/// Region is from <see cref="RegionStartIndex"/> to, but not including, <see cref="CurrentIndex"/>.
		/// </summary>
		/// <param name="tokenType"></param>
		/// <returns></returns>
		private CssToken ConsumeRegionToToken(CssTokenType tokenType)
		{
			var result = ConsumeRegion();
			return new CssToken(result, tokenType);
		}

		/// <summary>
		/// Finalizes the current region to a <see cref="CssToken"/> and starts a new region.
		/// Region is from <see cref="RegionStartIndex"/> to <see cref="CurrentIndex"/> (inclusive).
		/// </summary>
		/// <param name="tokenType"></param>
		/// <returns></returns>
		private CssToken ConsumeCurrentToToken(CssTokenType tokenType)
		{
			ConsumeCurrent();
			return ConsumeRegionToToken(tokenType);
		}

		/// <summary>
		/// Consumes the <see cref="Current"/> value.
		/// </summary>
		private void ConsumeCurrent() => ++IndexOffset;
		/// <summary>
		/// Consumes the next <paramref name="amount"/> number of items.
		/// </summary>
		/// <param name="amount"></param>
		private void ConsumeNext(int amount) => IndexOffset += amount;

		/// <summary>
		/// Push the current input code point back onto the front of the input stream, 
		/// so that the next time you are instructed to consume the next input code point, 
		/// it will instead reconsume the current input code point.
		/// </summary>
		private void Reconsume() => --IndexOffset;

		/// <summary>
		/// Applies the index offset to the primary index, completing the previous region and starting a new one.
		/// </summary>
		private void StartNewCaptureRegion()
		{
			RegionStartIndex += IndexOffset;
			IndexOffset = 0;
		}

		public bool NextToken(out CssToken token)
		{
			if (RegionStartIndex == InputStream.Length)
			{
				token = default;
				return false;
			}

			switch (Current)
			{
				case ' ':
				case '\t':
				case '\r':
				case '\n':
				case '\f':
					token = ConsumeWhitespaceToken();
					break;
				case '"':
				case '\'':
					token = ConsumeStringToToken();
					break;
				case '#':
					token = ConsumeNumberSignToToken();
					break;
				case '(':
					token = ConsumeCurrentToToken(CssTokenType.RoundBracketOpen);
					break;
				case ')':
					token = ConsumeCurrentToToken(CssTokenType.RoundBracketClose);
					break;
				case '+':
					token = ConsumePlusSignToToken();
					break;
				case ',':
					token = ConsumeCurrentToToken(CssTokenType.Comma);
					break;
				case '-':
					token = ConsumeHyphenMinusToToken();
					break;
				case '.':
					// If the input stream starts with a number, reconsume the current input code point,
					// consume a numeric token, and return it. 
					if (IsStartOfNumber())
					{
						token = ConsumeNumberToToken();
						break;
					}
					token = ConsumeCurrentToToken(CssTokenType.Delimiter);
					break;
				case ':':
					token = ConsumeCurrentToToken(CssTokenType.Colon);
					break;
				case ';':
					token = ConsumeCurrentToToken(CssTokenType.Semicolon);
					break;
				case '<':
					ConsumeCurrent();
					// If the next 3 input code points are U+0021 EXCLAMATION MARK U+002D HYPHEN-MINUS U+002D HYPHEN-MINUS (!--),
					// consume them and return a <CDO-token>. 
					if (Current == '!')
					{
						ConsumeCurrent();
						if (Current == '-')
						{
							ConsumeCurrent();
							if (Current == '-')
							{
								token = ConsumeCurrentToToken(CssTokenType.CDO);
								break;
							}
							Reconsume();
						}
						Reconsume();
					}
					token = ConsumeRegionToToken(CssTokenType.Delimiter);
					break;
				case '@':
					token = ConsumeCommercialAtToToken();
					break;
				case '[':
					token = ConsumeCurrentToToken(CssTokenType.SquareBracketOpen);
					break;
				case '\\':
					if (IsValidEscapeSequence())
					{
						token = ConsumeIdentifierLikeToToken();
						break;
					}
					token = ConsumeCurrentToToken(CssTokenType.Delimiter);
					break;
				case ']':
					token = ConsumeCurrentToToken(CssTokenType.SquareBracketClose);
					break;
				case '{':
					token = ConsumeCurrentToToken(CssTokenType.CurlyBracketOpen);
					break;
				case '}':
					token = ConsumeCurrentToToken(CssTokenType.CurlyBracketClose);
					break;
				default:
					if (char.IsDigit(Current))
					{
						token = ConsumeNumberToToken();
					}
					else if (IsValidIdentifierStartCodePoint())
					{
						token = ConsumeIdentifierLikeToToken();
					}
					else
					{
						token = ConsumeCurrentToToken(CssTokenType.Delimiter);
					}
					break;
			}
			return true;
		}

		/// <summary>
		/// <a href="https://drafts.csswg.org/css-syntax/#consume-token">4.3.1. Consume a token</a>
		/// </summary>
		/// <remarks>
		/// Consume as much whitespace as possible. Return a <whitespace-token>.
		/// </remarks>
		/// <returns></returns>
		private CssToken ConsumeWhitespaceToken()
		{
			while (true)
			{
				ConsumeCurrent();
				switch (Current)
				{
					case ' ':
					case '\t':
					case '\r':
					case '\n':
					case '\f':
						continue;
					default:
						return ConsumeRegionToToken(CssTokenType.Whitespace);
				}
			}
		}

		/// <summary>
		/// <a href="https://drafts.csswg.org/css-syntax/#consume-string-token">4.3.5. Consume a string token</a>
		/// </summary>
		/// <remarks>
		/// Consume a string token and return it. 
		/// </remarks>
		/// <returns></returns>
		private CssToken ConsumeStringToToken()
		{
			var quoteChar = Current;

			// Repeatedly consume the next input code point from the stream:
			while (true)
			{
				ConsumeCurrent();

				// Ending code point - return the <string-token>
				if (Current == quoteChar)
				{
					goto EndFound;
				}

				switch (Current)
				{
					case EndOfFile:
						// This is a parse error. Return the <string-token>.
						goto EndFound;
					case '\n':
						// This is a parse error. Reconsume the current input code point,
						// create a <bad-string-token>, and return it. 
						return new CssToken(CssTokenType.BadString);
					case '\\':
						ConsumeCurrent();
						switch (Current)
						{
							case EndOfFile:
								goto EndFound;
							case '\n':
								continue;
							default:
								ConsumeEscapeSequence();
								break;
						}
						break;
				}
			}

			EndFound:
			return ConsumeRegionToToken(CssTokenType.String);
		}

		/// <summary>
		/// <a href="https://drafts.csswg.org/css-syntax/#consume-token">4.3.1. Consume a token</a>
		/// </summary>
		/// <remarks>
		/// U+0023 NUMBER SIGN (#)
		/// </remarks>
		/// <returns></returns>
		private CssToken ConsumeNumberSignToToken()
		{
			ConsumeCurrent();

			// If the next input code point is an identifier code point or
			// the next two input code points are a valid escape, then: 
			if (IsValidIdentifierCodePoint() || IsValidEscapeSequence())
			{
				// If the next 3 input code points would start an identifier,
				// set the <hash-token>’s type flag to "id".
				var flags = CssTokenFlag.Hash_Unrestricted;
				if (IsStartOfIdentifier())
				{
					flags = CssTokenFlag.Hash_Id;
				}

				// Consume an identifier, and set the <hash-token>’s value to the returned string.
				// Return the <hash-token>.
				var identifier = ConsumeIdentifier();
				return new CssToken(identifier, CssTokenType.Hash, flags);
			}

			// Otherwise, return a <delim-token> with its value set to the current input code point.
			return ConsumeRegionToToken(CssTokenType.Delimiter);
		}

		/// <summary>
		/// <a href="https://drafts.csswg.org/css-syntax/#consume-token">4.3.1. Consume a token</a>
		/// </summary>
		/// <remarks>
		/// U+002B PLUS SIGN (+)
		/// </remarks>
		/// <returns></returns>
		private CssToken ConsumePlusSignToToken()
		{
			ConsumeCurrent();

			// If the input stream starts with a number, reconsume the current input code point,
			// consume a numeric token, and return it. 
			if (IsStartOfNumber())
			{
				return ConsumeNumberToToken();
			}

			// Otherwise, return a <delim-token> with its value set to the current input code point.
			return ConsumeRegionToToken(CssTokenType.Delimiter);
		}

		/// <summary>
		/// <a href="https://drafts.csswg.org/css-syntax/#consume-token">4.3.1. Consume a token</a>
		/// </summary>
		/// <remarks>
		/// U+002D HYPHEN-MINUS (-)
		/// </remarks>
		/// <returns></returns>
		private CssToken ConsumeHyphenMinusToToken()
		{
			ConsumeCurrent();

			// If the input stream starts with a number, reconsume the current input code point,
			// consume a numeric token, and return it. 
			if (IsStartOfNumber())
			{
				return ConsumeNumberToToken();
			}
			// Otherwise, if the next 2 input code points are U+002D HYPHEN-MINUS U+003E GREATER-THAN SIGN (->),
			// consume them and return a <CDC-token>.
			else if (Current == '-')
			{
				ConsumeCurrent();
				if (Current == '>')
				{
					ConsumeCurrent();
					return ConsumeCurrentToToken(CssTokenType.CDC);
				}
				Reconsume();
			}

			// Otherwise, if the input stream starts with an identifier, reconsume the current input code point,
			// consume an ident-like token, and return it.
			if (IsValidIdentifierStartCodePoint())
			{
				return ConsumeIdentifierLikeToToken();
			}

			// Otherwise, return a <delim-token> with its value set to the current input code point.
			return ConsumeRegionToToken(CssTokenType.Delimiter);
		}

		/// <summary>
		/// <a href="https://drafts.csswg.org/css-syntax/#consume-token">4.3.1. Consume a token</a>
		/// </summary>
		/// <remarks>
		/// U+0040 COMMERCIAL AT (@)
		/// </remarks>
		/// <returns></returns>
		private CssToken ConsumeCommercialAtToToken()
		{
			ConsumeCurrent();

			// If the next 3 input code points would start an identifier, consume an identifier,
			// create an <at-keyword-token> with its value set to the returned value, and return it. 
			if (IsStartOfIdentifier())
			{
				var identifier = ConsumeIdentifier();
				return new CssToken(identifier, CssTokenType.AtKeyword);
			}
			// Otherwise, return a <delim-token> with its value set to the current input code point.
			return ConsumeRegionToToken(CssTokenType.Delimiter);
		}

		/// <summary>
		/// <a href="https://drafts.csswg.org/css-syntax/#consume-a-numeric-token">4.3.3. Consume a numeric token</a>
		/// </summary>
		/// <returns></returns>
		private CssToken ConsumeNumberToToken()
		{
			// Consume a number and let number be the result.
			ConsumeNumber(out var number, out var flags);

			// If the next 3 input code points would start an identifier, then:
			if (IsStartOfIdentifier())
			{
				// Consume an identifier.
				var identifier = ConsumeIdentifier();
				// Create a <dimension-token> with the same value and type flag as number,
				// and a unit set to the identifier. 
				return new CssToken(number, CssTokenType.Dimension, flags, identifier);
			}
			// Otherwise, if the next input code point is U+0025 PERCENTAGE SIGN (%), consume it.
			// Create a <percentage-token> with the same value and type flag as number, and return it.
			else if (Current == '%')
			{
				ConsumeCurrent();
				return new CssToken(number, CssTokenType.Percentage, flags);
			}

			return new CssToken(number, CssTokenType.Number, flags);
		}

		/// <summary>
		/// <a href="https://drafts.csswg.org/css-syntax/#consume-ident-like-token">4.3.4. Consume an ident-like token</a>
		/// </summary>
		/// <remarks>
		/// Assumes that <see cref="Current"/> is the start of the identifier.
		/// </remarks>
		/// <returns></returns>
		private CssToken ConsumeIdentifierLikeToToken()
		{
			var identifier = ConsumeIdentifier();

			// If identifier matches "url" (case-insensitive) and the next input code point is U+0028 LEFT PARENTHESIS, consume it
			if (identifier.Equals("url", StringComparison.OrdinalIgnoreCase) && Current == '(')
			{
				ConsumeCurrent();
				// While the next two input code points are whitespace, consume the next input code point
				if (IsWhitespace())
				{
					ConsumeCurrent();

					// If the next one or two input code points are U+0022 QUOTATION MARK ("), U+0027 APOSTROPHE ('),
					// or whitespace followed by U+0022 QUOTATION MARK (") or U+0027 APOSTROPHE ('),
					// then create a <function-token> with its value set to string and return it.
					if (
						Current == '"' || Current == '\'' ||
						(Current == ' ' && (Peek() == '"' || Peek() == '\''))
					)
					{
						return new CssToken(identifier, CssTokenType.Function);
					}
				}

				return ConsumeUrlToToken();
			}
			// Otherwise, if the next input code point is U+0028 LEFT PARENTHESIS ((), consume it.
			// Create a <function-token> with its value set to string and return it.
			else if (Current == '(')
			{
				ConsumeCurrent();
				return new CssToken(identifier, CssTokenType.Function);
			}
			// Otherwise, create an <ident-token> with its value set to string and return it.
			else
			{
				return new CssToken(identifier, CssTokenType.Identifier);
			}
		}

		/// <summary>
		/// <a href="https://drafts.csswg.org/css-syntax/#consume-url-token">4.3.6. Consume a url token</a>
		/// </summary>
		/// <returns></returns>
		private CssToken ConsumeUrlToToken()
		{
			// Consume as much whitespace as possible
			while (true)
			{
				ConsumeCurrent();
				if (IsWhitespace())
				{
					continue;
				}
				break;
			}

			StartNewCaptureRegion();

			// Repeatedly consume the next input code point
			while (true)
			{
				if (Current == ')')
				{
					return ConsumeRegionToToken(CssTokenType.Url);
				}
				else if (Current == EndOfFile)
				{
					// This is a parse error BUT still returns the <url-token>
					return ConsumeRegionToToken(CssTokenType.Url);
				}
				else if (IsWhitespace())
				{
					// Consume as much whitespace as possible
					while (true)
					{
						ConsumeCurrent();
						if (IsWhitespace())
						{
							continue;
						}
						break;
					}

					if (Current == ')' || Current == EndOfFile)
					{
						// If EOF was encountered, this is a parse error BUT still return the <url-token>
						return ConsumeRegionToToken(CssTokenType.Url);
					}
					return ConsumeRemnantsOfBadUrlToToken();
				}
				else if (Current == '"' || Current == '\'' || Current == '(' || IsNonPrintableCodePoint())
				{
					// This is a parse error
					return ConsumeRemnantsOfBadUrlToToken();
				}
				else if (Current == '\\')
				{
					if (IsValidEscapeSequence())
					{
						ConsumeEscapeSequence();
						continue;
					}
					else
					{
						// This is a parse error
						return ConsumeRemnantsOfBadUrlToToken();
					}
				}

				// Append the current input code point to the <url-token>'s value
				ConsumeCurrent();
			}
		}

		/// <summary>
		/// <a href="https://drafts.csswg.org/css-syntax/#consume-remnants-of-bad-url">4.3.14. Consume the remnants of a bad url</a>
		/// </summary>
		/// <returns></returns>
		private CssToken ConsumeRemnantsOfBadUrlToToken()
		{
			while (true)
			{
				//TODO: May be able to optimize this using IndexOf('\') - if found, jump to it
				//		If not found, IndexOf(')').
				//		If not found, jump to end.
				//		May also not be worth optimizing as it is the exceptional case
				ConsumeCurrent();
				if (Current == ')')
				{
					ConsumeCurrent();
					return new CssToken(Span<char>.Empty, CssTokenType.BadUrl);
				}
				else if (Current == EndOfFile)
				{
					return new CssToken(Span<char>.Empty, CssTokenType.BadUrl);
				}
				else if (Current == '\\')
				{
					// The next character is escaped - doesn't matter how we handle it
					ConsumeCurrent();
				}
			}
		}

		/// <summary>
		/// <a href="https://drafts.csswg.org/css-syntax/#consume-number">4.3.12. Consume a number</a>
		/// </summary>
		/// <param name="seenDecimal"></param>
		/// <returns></returns>
		private void ConsumeNumber(out ReadOnlySpan<char> number, out CssTokenFlag flags)
		{
			var startIndex = CurrentIndex;
			var endIndex = startIndex;

			// Initially set type to "integer".
			flags = CssTokenFlag.Number_Integer;

			// If the next input code point is U+002B PLUS SIGN (+) or U+002D HYPHEN-MINUS (-),
			// consume it
			if (Current == '+' || Current == '-')
			{
				ConsumeCurrent();
			}

			// While the next input code point is a digit, consume it
			while (char.IsDigit(Current))
			{
				ConsumeCurrent();
			}

			// If the next 2 input code points are U+002E FULL STOP (.) followed by a digit, then:
			// - Consume them
			// - Set type to "number"
			if (Current == '.' && char.IsDigit(Peek()))
			{
				ConsumeNext(2);
				flags = CssTokenFlag.Number_Number;

				// While the next input code point is a digit, consume it
				while (char.IsDigit(Current))
				{
					ConsumeCurrent();
				}
			}

			// If the next 2 or 3 input code points are U+0045 LATIN CAPITAL LETTER E (E) or
			// U+0065 LATIN SMALL LETTER E (e), optionally followed by U+002D HYPHEN-MINUS (-) or
			// U+002B PLUS SIGN (+), followed by a digit, then: 
			if (
				(Current == 'E' || Current == 'e') &&
				(Peek() == '-' || Peek() == '+') &&
				char.IsDigit(Peek(2))
			)
			{
				ConsumeNext(3);
				flags = CssTokenFlag.Number_Number;

				// While the next input code point is a digit, consume it
				while (char.IsDigit(Current))
				{
					ConsumeCurrent();
				}
			}

			number = InputStream[startIndex..CurrentIndex];
		}

		/// <summary>
		/// <a href="https://drafts.csswg.org/css-syntax/#consume-name">4.3.11. Consume an identifier</a>
		/// </summary>
		/// <remarks>
		/// Assumes that <see cref="Current"/> is the start of the identifier.
		/// </remarks>
		/// <returns></returns>
		private ReadOnlySpan<char> ConsumeIdentifier()
		{
			var startIndex = CurrentIndex;
			while (true)
			{
				ConsumeCurrent();
				if (IsValidIdentifierCodePoint())
				{
					continue;
				}
				else if (IsValidEscapeSequence())
				{
					ConsumeEscapeSequence();
					continue;
				}
				return InputStream[startIndex..CurrentIndex];
			}
		}

		/// <summary>
		/// <a href="https://drafts.csswg.org/css-syntax/#whitespace">4.2. Definitions (Whitespace)</a>
		/// </summary>
		/// <returns></returns>
		private bool IsWhitespace()
		{
			return Current switch
			{
				' ' or '\t' or '\r' or '\n' or '\f' => true,
				_ => false,
			};
		}

		/// <summary>
		/// <a href="https://drafts.csswg.org/css-syntax/#starts-with-a-number">4.3.10. Check if three code points would start a number</a>
		/// </summary>
		/// <returns></returns>
		private bool IsStartOfNumber()
		{
			// Look at the first (current) code point:
			switch (Current)
			{
				case '+':
				case '-':
					// If the second code point is a digit, return true. 
					if (char.IsDigit(Peek()))
					{
						return true;
					}
					// Otherwise, if the second code point is a U+002E FULL STOP (.) and
					// the third code point is a digit, return true.
					// Otherwise, return false.
					return Peek() == '.' && char.IsDigit(Peek(2));
				case '.':
					// If the second code point is a digit, return true. Otherwise, return false.
					return char.IsDigit(Peek());
				default:
					// Digit, return true. Anything else, return false.
					return char.IsDigit(Current);
			}
		}

		/// <summary>
		/// <a href="https://drafts.csswg.org/css-syntax/#would-start-an-identifier">4.3.9. Check if three code points would start an identifier</a>
		/// </summary>
		/// <returns></returns>
		private bool IsStartOfIdentifier()
		{
			if (Current == '-')
			{
				ConsumeCurrent();
				// If the second code point is an identifier-start code point or a U+002D HYPHEN-MINUS,
				// or the second and third code points are a valid escape, return true.
				if (IsValidIdentifierStartCodePoint() || IsValidEscapeSequence())
				{
					Reconsume();
					return true;
				}
				// Otherwise, return false. 
				Reconsume();
				return false;
			}
			else if (IsValidIdentifierStartCodePoint())
			{
				return true;
			}
			else if (IsValidEscapeSequence())
			{
				return true;
			}
			return false;
		}

		/// <summary>
		/// <a href="https://drafts.csswg.org/css-syntax/#identifier-start-code-point">4.2. Definitions (Identifier-start code point)</a>
		/// </summary>
		/// <returns></returns>
		private bool IsValidIdentifierStartCodePoint()
		{
			const char CONTROL_CHARACTER = '\u0080';
			return char.IsLetter(Current) || Current == '_' || Current >= CONTROL_CHARACTER;
		}
		/// <summary>
		/// <a href="https://drafts.csswg.org/css-syntax/#identifier-code-point">4.2. Definitions (Identifier code point)</a>
		/// </summary>
		/// <returns></returns>
		private bool IsValidIdentifierCodePoint()
		{
			const char CONTROL_CHARACTER = '\u0080';
			return char.IsLetterOrDigit(Current) || Current == '-' || Current == '_' || Current >= CONTROL_CHARACTER;
		}

		private bool IsValidEscapeSequence()
		{
			if (Current == '\\')
			{
				ConsumeCurrent();
				if (Current != EndOfFile && Current != '\n')
				{
					Reconsume();
					return true;
				}
				Reconsume();
			}
			return false;
		}

		/// <summary>
		/// <a href="https://drafts.csswg.org/css-syntax/#non-printable-code-point">Non-printable code point</a>
		/// </summary>
		/// <returns></returns>
		private bool IsNonPrintableCodePoint()
		{
			// Between U+0000 NULL and U+0008 BACKSPACE (inclusive)
			if (Current >= 0x0000 && Current <= 0x0008)
			{
				return true;
			}
			// Either U+000B LINE TABULATION or U+007F DELETE
			else if (Current == 0x000B || Current == 0x007F)
			{
				return true;
			}
			// Between U+000E SHIFT OUT and U+001F INFORMATION SEPARATOR ONE (inclusive)
			else if (Current >= 0x000E && Current <= 0x001F)
			{
				return true;
			}
			return false;
		}

		/// <summary>
		/// Escape consumption here doesn't unescape the value - it just allows continuing after the escape sequence
		/// </summary>
		private void ConsumeEscapeSequence()
		{
			ConsumeCurrent();
			if (TokenizationHelper.IsHexDigit(Current))
			{
				var count = 0;
				while (count++ < 5)
				{
					ConsumeCurrent();
					if (TokenizationHelper.IsHexDigit(Current))
					{
						continue;
					}
					break;
				}

				if (Current != ' ')
				{
					Reconsume();
				}
			}
		}
	}
}
