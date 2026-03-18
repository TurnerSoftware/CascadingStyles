using System.Collections.Generic;
using TurnerSoftware.CascadingStyles.Parsing;

namespace TurnerSoftware.CascadingStyles.ObjectModel
{
	public class StyleRule : QualifiedRule
	{
		public Selector Selector
	}

	public struct StyleDeclaration
	{
		public readonly StringValue Value;
		public readonly bool IsImportant;

		public StyleDeclaration(StringValue value, bool isImportant)
		{
			Value = value;
			IsImportant = isImportant;
		}
	}
}
