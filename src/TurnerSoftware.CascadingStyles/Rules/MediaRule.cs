using System.Collections.Generic;

namespace TurnerSoftware.CascadingStyles.Rules
{
	public class MediaRule : ConditionRule
	{
		public ICollection<string> Media { get; } = new List<string>();
	}
}
