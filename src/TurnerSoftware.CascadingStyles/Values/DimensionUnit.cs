using System;

namespace TurnerSoftware.CascadingStyles.Values
{
	public class DimensionUnit : IEquatable<DimensionUnit>
	{
		public string FriendlyName { get; }
		public string UnitLiteral { get; }

		public override int GetHashCode() => UnitLiteral.GetHashCode();

		public override bool Equals(object obj)
		{
			if (obj is DimensionUnit unit)
			{
				return Equals(unit);
			}

			return false;
		}

		public bool Equals(DimensionUnit other)
		{
			if (other is null)
			{
				return false;
			}

			if (UnitLiteral == other.UnitLiteral)
			{
				return true;
			}

			return false;
		}


		public string ToString(decimal value) => $"{value}{UnitLiteral}";

		public static bool TryGetNumericUnit(ReadOnlySpan<char> name, out DimensionUnit unit)
		{
			for (var i = 0; i < KnownUnits.Length; i++)
			{
				var knownUnit = KnownUnits[i];
				if (name.Equals(knownUnit.UnitLiteral, StringComparison.OrdinalIgnoreCase))
				{
					unit = knownUnit;
					return true;
				}
			}

			unit = null;
			return false;
		}

		private static readonly DimensionUnit[] KnownUnits = new DimensionUnit[]
		{
			Pixel, Point,
			EM, REM,
			ViewportWidth, ViewportHeight,
			Second, Millisecond,
			Degree
		};

		public DimensionUnit(string friendlyName, string unitLiteral)
		{
			FriendlyName = friendlyName;
			UnitLiteral = unitLiteral;
		}

		public static DimensionUnit None { get; } = new DimensionUnit("None", string.Empty);

		public static DimensionUnit Percentage { get; } = new PercentageUnit();

		public static LengthUnit Pixel { get; } = new LengthUnit("Pixel", "px");
		public static LengthUnit Point { get; } = new LengthUnit("Point", "px");

		public static LengthUnit EM { get; } = new LengthUnit("EM", "em");
		public static LengthUnit REM { get; } = new LengthUnit("REM", "rem");

		public static LengthUnit ViewportWidth { get; } = new LengthUnit("Viewport Width", "vw");
		public static LengthUnit ViewportHeight { get; } = new LengthUnit("Viewport Height", "vh");

		public static TimeUnit Second { get; } = new TimeUnit("Second", "s");
		public static TimeUnit Millisecond { get; } = new TimeUnit("Second", "ms");

		public static AngleUnit Degree { get; } = new AngleUnit("Degree", "deg");
	}

	public class PercentageUnit : DimensionUnit
	{
		public PercentageUnit() : base("Percentage", "%") { }
	}

	public class LengthUnit : DimensionUnit
	{
		public LengthUnit(string friendlyName, string unitLiteral) : base(friendlyName, unitLiteral) { }
	}

	public class TimeUnit : DimensionUnit
	{
		public TimeUnit(string friendlyName, string unitLiteral) : base(friendlyName, unitLiteral) { }
	}

	public class AngleUnit : DimensionUnit
	{
		public AngleUnit(string friendlyName, string unitLiteral) : base(friendlyName, unitLiteral) { }
	}
}
