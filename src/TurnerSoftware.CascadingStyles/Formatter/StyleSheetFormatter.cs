using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using TurnerSoftware.CascadingStyles.Rules;

namespace TurnerSoftware.CascadingStyles.Formatter
{
	public class StyleSheetFormatter
	{
		public async ValueTask WriteAsync(StyleSheet styleSheet, TextWriter writer)
		{
			foreach (var rule in styleSheet.Rules)
			{
				await WriteRuleAsync(rule, writer);
			}
		}

		private static async ValueTask WriteRuleAsync(Rule rule, TextWriter writer)
		{
			if (rule is StyleRule styleRule)
			{
				await WriteStyleRuleAsync(styleRule, writer);
			}
			else if (rule is MediaRule mediaRule)
			{
				await writer.WriteAsync("@media ");
				await writer.WriteAsync(mediaRule.Condition);
				await WriteBlockAsync(mediaRule.Rules, writer);
			}
		}

		private static async ValueTask WriteStyleRuleAsync(StyleRule styleRule, TextWriter writer)
		{
			await writer.WriteAsync(styleRule.SelectorText);
			await writer.WriteAsync('{');

			foreach (var (property, value) in styleRule.Style)
			{
				await writer.WriteAsync(property);
				await writer.WriteAsync(':');
				await writer.WriteAsync(value);
				await writer.WriteAsync(';');
			}

			await writer.WriteAsync('}');
		}

		private static async ValueTask WriteBlockAsync(ICollection<Rule> rules, TextWriter writer)
		{
			writer.Write('{');

			foreach (var rule in rules)
			{
				await WriteRuleAsync(rule, writer);
			}

			writer.Write('}');
		}
	}
}
