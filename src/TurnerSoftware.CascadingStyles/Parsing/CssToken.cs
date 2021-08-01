using System;
using System.ComponentModel;

#pragma warning disable 0809 // Obsolete member overrides non-obsolete member

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

		public CssToken(ReadOnlyMemory<char> rawValue, CssTokenType type, CssTokenFlag flags = CssTokenFlag.None, ReadOnlyMemory<char> secondaryValue = default)
		{
			RawValue = rawValue;
			Type = type;
			Flags = flags;
			RawSecondaryValue = secondaryValue;
		}

		public static bool operator !=(CssToken left, CssToken right) => !(left == right);

		public static bool operator ==(CssToken left, CssToken right) =>
			left.RawValue.Equals(right) &&
			left.Type == right.Type &&
			left.Flags == right.Flags &&
			left.RawSecondaryValue.Equals(right);

		[Obsolete("Equals() on CssToken will always throw an exception. Use == instead.")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override bool Equals(object obj) => throw new NotSupportedException();
		[Obsolete("GetHashCode() on CssToken will always throw an exception.")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override int GetHashCode() => throw new NotSupportedException();
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
