using System.Collections.Generic;

namespace TurnerSoftware.CascadingStyles
{
	public static class SelectorExtensions
	{
		public static Selector PseudoClasses(this Selector selector, IEnumerable<string> pseudoClasses)
		{
			foreach (var pseudoClass in pseudoClasses)
			{
				selector.PseudoClass(pseudoClass);
			}

			return selector;
		}
	}
}
