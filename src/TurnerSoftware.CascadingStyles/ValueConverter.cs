using System;
using System.Text.RegularExpressions;
using TurnerSoftware.CascadingStyles.Values;

namespace TurnerSoftware.CascadingStyles
{
	public static class ValueConverter
	{
		private static readonly Regex NumericValueRegex = new(@"(?<value>-?\d*(\.\d+)?)(?<unit>[a-z]+)?", RegexOptions.IgnoreCase | RegexOptions.Compiled);
		
		public static NumericValue ToNumericValue(string value)
		{
			var result = NumericValueRegex.Match(value);

			if (!result.Groups.ContainsKey("value") || !decimal.TryParse(result.Groups["value"].Value, out var number))
			{
				throw new FormatException("Value does is not a valid numeric value");
			}

			if (!result.Groups.ContainsKey("unit") || !NumericUnitHelper.TryGetNumericUnit(result.Groups["unit"].Value, out var unit))
			{
				unit = NumericUnit.None;
			}

			return new NumericValue(number, unit);
		}
	}
}
