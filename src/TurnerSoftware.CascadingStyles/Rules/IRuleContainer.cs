using System.Collections.Generic;

namespace TurnerSoftware.CascadingStyles.Rules
{
	public interface IRuleContainer
	{
		ICollection<Rule> Rules { get; }
	}
}
