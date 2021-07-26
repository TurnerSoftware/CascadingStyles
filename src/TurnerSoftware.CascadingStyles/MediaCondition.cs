using System.Runtime.CompilerServices;
using System.Text;

namespace TurnerSoftware.CascadingStyles
{
	/// <summary>
	/// Allows construction of media conditions for a <see cref="Rules.MediaRule"/>.
	/// </summary>
	public class MediaCondition
	{
		public StringBuilder Builder { get; }

		private MediaCondition(string condition)
		{
			Builder = new StringBuilder(condition);
		}
		
		/// <summary>
		/// Returns a <see cref="MediaCondition"/> with the "all" media type set.
		/// </summary>
		public static MediaCondition All => new("all");
		/// <summary>
		/// Returns a <see cref="MediaCondition"/> with the "print" media type set.
		/// </summary>
		public static MediaCondition Print => new("print");
		/// <summary>
		/// Returns a <see cref="MediaCondition"/> with the "screen" media type set.
		/// </summary>
		public static MediaCondition Screen => new("screen");
		/// <summary>
		/// Returns a <see cref="MediaCondition"/> with the "speech" media type set.
		/// </summary>
		public static MediaCondition Speech => new("speech");

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private MediaCondition Append(string value)
		{
			Builder.Append(value);
			return this;
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private MediaCondition Append(char value)
		{
			Builder.Append(value);
			return this;
		}

		/// <summary>
		/// Appends the "all" media type to the current <see cref="MediaCondition"/>.
		/// </summary>
		public MediaCondition AllMedia => Append("all");
		/// <summary>
		/// Appends the "print" media type to the current <see cref="MediaCondition"/>.
		/// </summary>
		public MediaCondition PrintMedia => Append("print");
		/// <summary>
		/// Appends the "screen" media type to the current <see cref="MediaCondition"/>.
		/// </summary>
		public MediaCondition ScreenMedia => Append("screen");
		/// <summary>
		/// Appends the "speech" media type to the current <see cref="MediaCondition"/>.
		/// </summary>
		public MediaCondition SpeechMedia => Append("speech");

		/// <summary>
		/// Appends the logical "and" to the current <see cref="MediaCondition"/>.
		/// </summary>
		public MediaCondition And => Append(" and ");
		/// <summary>
		/// Appends the logical "or" (a comma) to the current <see cref="MediaCondition"/>.
		/// </summary>
		public MediaCondition Also => Append(',');

		/// <summary>
		/// Appends the specified featured to the current <see cref="MediaCondition"/> including the surrounding parentheses.
		/// </summary>
		/// <param name="feature"></param>
		/// <returns></returns>
		public MediaCondition Feature(string feature)
		{
			Builder.Append('(');
			Builder.Append(feature);
			Builder.Append(')');
			return this;
		}
		/// <summary>
		/// Appends the specified feature by <paramref name="name"/> and <paramref name="value"/> to the current <see cref="MediaCondition"/> including the surrounding parentheses.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public MediaCondition Feature(string name, string value)
		{
			Builder.Append('(');
			Builder.Append(name);
			Builder.Append(':');
			Builder.Append(value);
			Builder.Append(')');
			return this;
		}

		/// <summary>
		/// Returns the current <see cref="MediaCondition"/> as a complete string.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return Builder.ToString();
		}

		public static implicit operator string(MediaCondition condition)
		{
			return condition.ToString();
		}
	}
}
