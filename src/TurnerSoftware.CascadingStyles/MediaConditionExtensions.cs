using System;
using TurnerSoftware.CascadingStyles.Values;

namespace TurnerSoftware.CascadingStyles
{
	public static class MediaConditionExtensions
	{
		/// <summary>
		/// Appends the "min-width" feature to the current <see cref="MediaCondition"/> with the specified <paramref name="value"/>.
		/// </summary>
		/// <param name="condition"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public static MediaCondition MinWidth(this MediaCondition condition, Dimension value)
		{
			if (value.Unit is not LengthUnit)
			{
				throw new ArgumentException("Value is not a length unit", nameof(value));
			}
			
			return condition.Feature("min-width", value);
		}
	}
}
