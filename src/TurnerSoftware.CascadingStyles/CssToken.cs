using System;
using System.Text;

namespace TurnerSoftware.CascadingStyles
{
	public ref struct CssToken
	{
		public ReadOnlySpan<char> RawValue;
		public CssTokenType Type;
		public CssTokenFlag Flags;
		public ReadOnlySpan<char> RawSecondaryValue;

		public CssToken(CssTokenType type)
		{
			RawValue = default;
			Type = type;
			Flags = CssTokenFlag.None;
			RawSecondaryValue = default;
		}

		public CssToken(ReadOnlySpan<char> rawValue, CssTokenType type, CssTokenFlag flags = CssTokenFlag.None, ReadOnlySpan<char> secondaryValue = default)
		{
			RawValue = rawValue;
			Type = type;
			Flags = flags;
			RawSecondaryValue = secondaryValue;
		}
	}

	public enum CssTokenType
	{
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
		SquareBracketOpen,
		SquareBracketClose,
		RoundBracketOpen,
		RoundBracketClose,
		CurlyBracketOpen,
		CurlyBracketClose
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
