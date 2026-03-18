using System.Collections.Generic;
using TurnerSoftware.CascadingStyles.Parsing;

namespace TurnerSoftware.CascadingStyles.ObjectModel
{
	public class StyleSheet
	{
		public List<ICssRule> Rules { get; init; } = new List<ICssRule>();
	}
}
