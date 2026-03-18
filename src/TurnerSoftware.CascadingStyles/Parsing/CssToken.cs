using System;

namespace TurnerSoftware.CascadingStyles.Parsing
{
	public struct CssToken
	{
		public readonly ReadOnlyMemory<char> RawValue;
		public readonly CssTokenType Type;
		public readonly CssTokenFlag Flags;
		public readonly ReadOnlyMemory<char> RawSecondaryValue;

		public static CssToken EndOfFile => new(CssTokenType.EndOfFile);

		public CssToken(CssTokenType type)
		{
			RawValue = default;
			Type = type;
			Flags = CssTokenFlag.None;
			RawSecondaryValue = default;
		}

		public CssToken(string rawValue, CssTokenType type, CssTokenFlag flags = CssTokenFlag.None, string secondaryValue = default)
		{
			RawValue = rawValue.AsMemory();
			Type = type;
			Flags = flags;
			RawSecondaryValue = secondaryValue.AsMemory();
		}

		public CssToken(ReadOnlyMemory<char> rawValue, CssTokenType type, CssTokenFlag flags = CssTokenFlag.None, ReadOnlyMemory<char> secondaryValue = default)
		{
			RawValue = rawValue;
			Type = type;
			Flags = flags;
			RawSecondaryValue = secondaryValue;
		}

		public static bool operator !=(CssToken left, CssToken right) => !(left == right);

		public static bool operator ==(CssToken left, CssToken right) =>
			left.RawValue.Span.Equals(right.RawValue.Span, StringComparison.OrdinalIgnoreCase) &&
			left.Type == right.Type &&
			left.Flags == right.Flags &&
			left.RawSecondaryValue.Span.Equals(right.RawSecondaryValue.Span, StringComparison.OrdinalIgnoreCase);

		public override bool Equals(object obj) => obj is CssToken token && 
			this == token;
		
		public override int GetHashCode() => HashCode.Combine(RawValue, Type, Flags, RawSecondaryValue);

		public override string ToString()
		{
			return Type switch
			{
				CssTokenType.Delimiter or CssTokenType.Number => RawValue.ToString(),
				CssTokenType.AtKeyword => "@",
				CssTokenType.Colon => ":",
				CssTokenType.Semicolon => ";",
				CssTokenType.Comma => ",",
				CssTokenType.String => $"\"{RawValue}\"",
				CssTokenType.Dimension => $"{RawValue}{RawSecondaryValue}",
				CssTokenType.Percentage => "%",
				CssTokenType.Identifier => RawValue.ToString(),
				CssTokenType.LeftCurlyBracket => "{",
				CssTokenType.RightCurlyBracket => "}",
				CssTokenType.LeftSquareBracket => "[",
				CssTokenType.RightSquareBracket => "]",
				CssTokenType.LeftParenthesis => "(",
				CssTokenType.RightParenthesis => ")",
				CssTokenType.Whitespace => RawValue.ToString(),
				CssTokenType.Hash => "#",
				_ => string.Empty
			};
		}
	}

	public enum CssTokenType
	{
		EndOfFile,
		Identifier,
		Function,
		AtKeyword,
		Hash,
		String,
		BadString,
		Url,
		BadUrl,
		Delimiter,
		Number,
		Percentage,
		Dimension,
		Whitespace,
		CDO,
		CDC,
		Colon,
		Semicolon,
		Comma,
		LeftSquareBracket,
		RightSquareBracket,
		LeftParenthesis,
		RightParenthesis,
		LeftCurlyBracket,
		RightCurlyBracket
	}

	public enum CssTokenFlag
	{
		/// <summary>
		/// No token flag is set
		/// </summary>
		None,
		/// <summary>
		/// The default flag for &lt;hash-token&gt;.
		/// </summary>
		Hash_Unrestricted,
		/// <summary>
		/// Defines that the &lt;hash-token&gt; is a valid ID.
		/// </summary>
		Hash_Id,
		/// <summary>
		/// Whether a &lt;number-token> or &lt;dimension&gt; is a whole number.
		/// </summary>
		Number_Integer,
		/// <summary>
		/// Whether a &lt;number-token> or &lt;dimension&gt; is a fraction.
		/// </summary>
		Number_Number
	}
}
