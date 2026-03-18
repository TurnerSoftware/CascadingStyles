using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TurnerSoftware.CascadingStyles.Formatter;
using TurnerSoftware.CascadingStyles.ObjectModel;

namespace TurnerSoftware.CascadingStyles.Parsing
{
	public abstract class Rule : ICssRule
	{
		public List<ComponentValue> Prelude { get; init; } = new List<ComponentValue>();
		public SimpleBlock Value { get; set; }
		public void Add(ComponentValue component)
		{
			Prelude.Add(component);
		}

		public override bool Equals(object obj)
		{
			if (obj is Rule rule)
			{
				return Prelude.SequenceEqual(rule.Prelude) &&
					Value.Equals(rule.Value);
			}

			return false;
		}

		public override int GetHashCode() => HashCode.Combine(Prelude, Value);

		public override string ToString()
		{
			var builder = new StringBuilder();

			foreach (var component in Prelude)
			{
				builder.Append(component.ToString());
			}

			if (Value is not null)
			{
				builder.Append(Value.ToString());
			}

			return builder.ToString();
		}
	}

	public class AtRule : Rule
	{
		public StringValue Name { get; set; }

		public void WriteTo(TextWriter writer, StyleSheetFormatter formatter)
		{
			writer.Write('@');
			writer.Write(Name);

			if (Prelude is not null)
			{
				foreach (var item in Prelude)
				{
					writer.Write(item.ToString());
				}
			}

			if (Value is null)
			{
				writer.Write(';');
			}
			else
			{
				writer.Write(Value.ToString());
			}
		}

		public override bool Equals(object obj)
		{
			if (obj is AtRule atRule)
			{
				return Name == atRule.Name &&
					base.Equals(obj);
			}

			return false;
		}

		public override int GetHashCode() => HashCode.Combine(Name, base.GetHashCode());

		public override string ToString()
		{
			var builder = new StringBuilder();

			builder.Append('@');
			builder.Append((string)Name);
			builder.Append(base.ToString());

			return builder.ToString();
		}
	}

	public class QualifiedRule : Rule { }

	public abstract class ComponentValue { }

	public class SimpleBlock : ComponentValue, IEnumerable<ComponentValue>
	{
		public SimpleBlockType Type { get; set; }
		public List<ComponentValue> Value { get; init; } = new List<ComponentValue>();

		public SimpleBlock() { }
		public SimpleBlock(SimpleBlockType type)
		{
			Type = type;
		}

		public override bool Equals(object obj)
		{
			if (obj is SimpleBlock simpleBlock)
			{
				return Type == simpleBlock.Type &&
					Value.SequenceEqual(simpleBlock.Value);
			}

			return false;
		}

		public void Add(ComponentValue component)
		{
			Value.Add(component);
		}

		public override int GetHashCode() => HashCode.Combine(Type, Value);

		public override string ToString()
		{
			var builder = new StringBuilder();

			builder.Append(Type switch
			{
				SimpleBlockType.SquareBracket => '[',
				SimpleBlockType.Parenthesis => '(',
				SimpleBlockType.CurlyBracket => '{',
				_ => throw new InvalidOperationException("Unknown block type")
			});

			foreach (var component in Value)
			{
				builder.Append(component.ToString());
			}

			builder.Append(Type switch
			{
				SimpleBlockType.SquareBracket => ']',
				SimpleBlockType.Parenthesis => ')',
				SimpleBlockType.CurlyBracket => '}',
				_ => throw new InvalidOperationException("Unknown block type")
			});

			return builder.ToString();
		}

		public IEnumerator<ComponentValue> GetEnumerator() => Value.GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}
	public enum SimpleBlockType
	{
		SquareBracket,
		Parenthesis,
		CurlyBracket
	}

	public class CssFunction : ComponentValue
	{
		public StringValue Name { get; set; }
		public List<ComponentValue> Value { get; init; } = new List<ComponentValue>();

		public override bool Equals(object obj)
		{
			if (obj is CssFunction cssFunction)
			{
				return Name == cssFunction.Name &&
					Value.SequenceEqual(cssFunction.Value);
			}

			return false;
		}

		public override int GetHashCode() => HashCode.Combine(Name, Value);

		public override string ToString()
		{
			var builder = new StringBuilder(Name);
			builder.Append('(');

			foreach (var component in Value)
			{
				builder.Append(component.ToString());
			}

			builder.Append(')');
			return builder.ToString();
		}
	}

	public class TokenComponent : ComponentValue
	{
		public CssToken Token { get; set; }

		public override bool Equals(object obj)
		{
			if (obj is TokenComponent component)
			{
				return Token.Equals(component.Token);
			}

			return false;
		}

		public override int GetHashCode() => Token.GetHashCode();

		public override string ToString() => Token.ToString();
	}
}
