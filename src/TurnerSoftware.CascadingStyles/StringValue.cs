using System;
using System.Runtime.CompilerServices;

namespace TurnerSoftware.CascadingStyles
{
	public struct StringValue
	{
		private readonly ReadOnlyMemory<char> Value;

		public static readonly StringValue Empty = new();

		private StringValue(char value)
		{
			Value = new[] { value }.AsMemory();
		}

		private StringValue(string value)
		{
			Value = value.AsMemory();
		}

		private StringValue(ReadOnlyMemory<char> value)
		{
			Value = value;
		}

		public bool IsEmpty => Value.IsEmpty;

		public static implicit operator StringValue(char value) => new(value);

		public static implicit operator StringValue(string value) => new(value);

		public static implicit operator string(StringValue value) => value.ToString();

		public static implicit operator StringValue(ReadOnlyMemory<char> value) => new(value);

		public static implicit operator ReadOnlyMemory<char>(StringValue value) => value.Value;

		public override bool Equals(object obj)
		{
			if (obj is StringValue stringValue)
			{
				return Equals(stringValue, StringComparison.Ordinal);
			}

			return false;
		}

		public bool Equals(StringValue stringValue, StringComparison comparison) => Value.Span.Equals(stringValue.Value.Span, comparison);

		public static bool operator ==(StringValue left, StringValue right) => left.Equals(right, StringComparison.Ordinal);

		public static bool operator !=(StringValue left, StringValue right) => !(left == right);

		public override int GetHashCode() => Value.GetHashCode();

		public override string ToString() => Value.ToString();
	}
}