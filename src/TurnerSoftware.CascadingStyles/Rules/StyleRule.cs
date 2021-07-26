using System;
using System.Collections.Generic;

namespace TurnerSoftware.CascadingStyles.Rules
{
	public class StyleRule : Rule
	{
		public string SelectorText { get; set; }
		public Dictionary<string, string> Style { get; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
	}
}
