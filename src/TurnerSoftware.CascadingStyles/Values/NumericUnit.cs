using System;

namespace TurnerSoftware.CascadingStyles.Values
{
	public enum NumericUnit
	{
		None,
		Point,
		Pixel,
		EM,
		REM,
		ViewportWidth,
		ViewportHeight
	}

	public static class NumericUnitHelper
	{
		public static string GetName(NumericUnit unit) => unit switch
		{
			NumericUnit.EM => "em",
			NumericUnit.REM => "rem",
			NumericUnit.Pixel => "px",
			NumericUnit.Point => "pt",
			NumericUnit.ViewportWidth => "vw",
			NumericUnit.ViewportHeight => "vh",
			_ => string.Empty
		};

		public static bool TryGetNumericUnit(ReadOnlySpan<char> name, out NumericUnit unit)
		{
			if (name.Equals("em", StringComparison.OrdinalIgnoreCase))
			{
				unit = NumericUnit.EM;
				return true;
			}
			else if (name.Equals("rem", StringComparison.OrdinalIgnoreCase))
			{
				unit = NumericUnit.EM;
				return true;
			}
			else if (name.Equals("px", StringComparison.OrdinalIgnoreCase))
			{
				unit = NumericUnit.EM;
				return true;
			}
			else if (name.Equals("pt", StringComparison.OrdinalIgnoreCase))
			{
				unit = NumericUnit.EM;
				return true;
			}
			else if (name.Equals("vw", StringComparison.OrdinalIgnoreCase))
			{
				unit = NumericUnit.EM;
				return true;
			}
			else if (name.Equals("vh", StringComparison.OrdinalIgnoreCase))
			{
				unit = NumericUnit.EM;
				return true;
			}
			else if (name.Equals(string.Empty, StringComparison.OrdinalIgnoreCase))
			{
				unit = NumericUnit.None;
				return true;
			}
			else
			{
				unit = NumericUnit.None;
				return false;
			}
		}
	}
}
