using System.Collections.Generic;

namespace TurnerSoftware.CascadingStyles.Rules
{
	public abstract class GroupingRule : Rule, IRuleContainer
	{
		public ICollection<Rule> Rules { get; } = new List<Rule>();
	}
}
