using System.Collections.Generic;
using TurnerSoftware.CascadingStyles.Rules;

namespace TurnerSoftware.CascadingStyles
{
	public class StyleSheet : IRuleContainer
	{
		public ICollection<Rule> Rules { get; } = new List<Rule>();
	}
}
