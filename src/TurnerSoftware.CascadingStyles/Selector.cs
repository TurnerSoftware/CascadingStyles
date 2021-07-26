using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;

namespace TurnerSoftware.CascadingStyles
{
	public class Selector
	{
		private StringBuilder Builder { get; } = new StringBuilder();

		private static readonly Regex InvalidClassNameCharacters = new("[^a-z0-9_\\-]", RegexOptions.IgnoreCase | RegexOptions.Compiled);

		public Selector @Class(string name)
		{
			Builder.Append('.');
			Builder.Append(InvalidClassNameCharacters.Replace(name, "\\$0"));
			return this;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private Selector Append(string value)
		{
			Builder.Append(value);
			return this;
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private Selector Append(char value)
		{
			Builder.Append(value);
			return this;
		}

		public Selector Type(string element) => Append(element);

		public Selector Universal() => Append('*');

		public Selector ID(string id)
		{
			Builder.Append('#');
			Builder.Append(id);
			return this;
		}

		public Selector Attribute(string attribute)
		{
			Builder.Append('[');
			Builder.Append(attribute);
			Builder.Append(']');
			return this;
		}

		public Selector Also => Append(',');

		public Selector Descendant => Append(' ');

		public Selector Child => Append('>');

		public Selector AnySibling => Append('~');

		public Selector AdjacentSibling => Append('+');

		public Selector PseudoClass(string name)
		{
			Builder.Append(':');
			Builder.Append(name);
			return this;
		}

		public Selector PseudoElement(string element)
		{
			Builder.Append(':').Append(':');
			Builder.Append(element);
			return this;
		}

		public override string ToString()
		{
			return Builder.ToString();
		}

		public static implicit operator string(Selector selector)
		{
			return selector.ToString();
		}
	}
}
