using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TurnerSoftware.CascadingStyles.ObjectModel;
using TurnerSoftware.CascadingStyles.Parsing;

namespace TurnerSoftware.CascadingStyles.Serialization
{
	public static class StyleSheetSerializer
	{
		public static StyleSheet Deserialize(ReadOnlyMemory<char> value)
		{
			var result = new StyleSheet();
			var parser = new CssTokenParser(new CssTokenizer(value));

			while (parser.NextRule(out var rule))
			{
				if (rule is QualifiedRule qualifiedRule)
				{
					result.Rules.Add(DeserializeStyleRule(qualifiedRule));
				}
				else if (rule is AtRule atRule)
				{
					result.Rules.Add(DeserializeAtRule(atRule));
				}
				else
				{
					throw new InvalidOperationException("Unknown rule type for deserialization");
				}
			}

			return result;
		}

		private static Rule DeserializeStyleRule(QualifiedRule rule)
		{
			var result = new StyleRule();

			result.Style
			//result.SelectorText =
			return result;
		}

		private static Rule DeserializeAtRule(AtRule rule)
		{
			throw new NotImplementedException();
		}
	}
}
