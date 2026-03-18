using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace TurnerSoftware.CascadingStyles.ObjectModel
{
	/*
	 * CSS Selectors Spec
	 * https://www.w3.org/TR/selectors/
	 */
	public struct Selector
	{
		public List<ISelectorComponent> Components { get; init; }

		public Selector(List<ISelectorComponent> components)
		{
			Components = components;
		}

		public 

		public Selector @Class(string name)
		{

			//Builder.Append(InvalidClassNameCharacters.Replace(name, "\\$0"));
			return this;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private Selector Append(string value)
		{

			return this;
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private Selector Append(char value)
		{

			return this;
		}

		public Selector Type(string element) => Append(element);

		public Selector Universal() => Append('*');

		public Selector ID(string id)
		{

			return this;
		}

		public Selector Attribute(string attribute)
		{

			return this;
		}

		public Selector Also => Append(',');

		public Selector Descendant => Append(' ');

		public Selector Child => Append('>');

		public Selector AnySibling => Append('~');

		public Selector AdjacentSibling => Append('+');

		public Selector PseudoClass(StringValue name)
		{

			return this;
		}

		public Selector PseudoElement(string element)
		{

			return this;
		}


		public override string ToString()
		{
			var builder = new StringBuilder();
			if (Components is not null)
			{
				for (var i = 0; i < Components.Count; i++)
				{
					builder.Append(Components[i].ToString());
				}
				builder.Remove(builder.Length - 1, 1);
			}
			return builder.ToString();
		}
	}

	public interface ISelectorComponent { }

	public sealed class SelectorUnit : ISelectorComponent
	{
		public static readonly SelectorUnit Universal = new() { TagName = "*" };

		public StringValue TagName { get; init; }
		public StringValue Id { get; init; }
		public List<StringValue> Classes { get; init; } = new();
		public List<AttributeSelector> Attributes { get; init; } = new();
		public List<PseudoSelector> Pseudo { get; init; } = new();

		public override string ToString()
		{
			var builder = new StringBuilder();

			builder.Append((ReadOnlyMemory<char>)TagName);

			if (!Id.IsEmpty)
			{
				builder.Append('#');
				builder.Append((ReadOnlyMemory<char>)Id);
			}
			
			if (Classes is not null)
			{
				for (var i = 0; i < Classes.Count; i++)
				{
					builder.Append('.');
					builder.Append((ReadOnlyMemory<char>)Classes[i]);
				}
			}

			if (Attributes is not null)
			{
				for (var i = 0; i < Attributes.Count; i++)
				{
					builder.Append(Attributes[i].ToString());
				}
			}

			if (Pseudo is not null)
			{
				for (var i = 0; i < Pseudo.Count; i++)
				{
					builder.Append(Pseudo[i].ToString());
				}
			}

			if (builder.Length == 0)
			{
				return string.Empty;
			}

			return builder.ToString();
		}
	}

	public sealed class AttributeSelector
	{
		public StringValue Attribute { get; }
		public StringValue Operator { get; }
		public StringValue Value { get; }
		public bool IsCaseSensitive { get; }

		public AttributeSelector(StringValue attribute, StringValue @operator, StringValue value, bool isCaseSensitive = true)
		{
			Attribute = attribute;
			Operator = @operator;
			Value = value;
			IsCaseSensitive = isCaseSensitive;
		}

		public override string ToString()
		{
			if (!Value.IsEmpty)
			{
				if (IsCaseSensitive)
				{
					return $"[{Attribute}{Operator}\"{Value}\"]";
				}
				else
				{
					return $"[{Attribute}{Operator}\"{Value}\" i]";
				}
			}
			else
			{
				return $"[{Attribute}]";
			}
		}
	}

	public sealed class SelectorCombinator : ISelectorComponent
	{
		public static SelectorCombinator Descendant => new(' ');
		public static SelectorCombinator Child => new('>');
		public static SelectorCombinator GeneralSibling => new('~');
		public static SelectorCombinator AdjacentSibling => new('~');

		private readonly StringValue Combinator;

		private SelectorCombinator(StringValue combinator)
		{
			Combinator = combinator;
		}

		public override string ToString() => Combinator;
	}

	public struct PseudoSelector
	{
		public readonly PseudoType Type;
		public readonly StringValue Value;

		public PseudoSelector(PseudoType type, StringValue value)
		{
			Type = type;
			Value = value;
		}

		public override string ToString() => Type switch
		{
			PseudoType.Class => $":{Value}",
			PseudoType.Element => $"::{Value}",
			_ => throw new InvalidOperationException("Invalid PseudoType")
		};
	}
	public enum PseudoType
	{
		Class,
		Element
	}

	public struct SelectorGroupSeparator : ISelectorComponent
	{
		private const string Value = ",";
		public override string ToString() => Value;
	}
}
