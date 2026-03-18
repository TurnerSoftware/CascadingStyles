using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TurnerSoftware.CascadingStyles.ObjectModel
{
	public static class StyleRuleExtensions
	{
		public static StyleRule AddStyles(this StyleRule styleRule, Dictionary<string, string> properties)
		{
			foreach (var (name, value) in properties)
			{
				//styleRule.Style.Add(name, value);
			}

			return styleRule;
		}
	}
}
