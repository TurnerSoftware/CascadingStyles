namespace TurnerSoftware.CascadingStyles.Values
{
	public struct NumericValue
	{
		public readonly decimal Number;
		public readonly NumericUnit Unit;

		public NumericValue(decimal number, NumericUnit unit)
		{
			Number = number;
			Unit = unit;
		}

		public override string ToString()
		{
			return Number + NumericUnitHelper.GetName(Unit);
		}
	}
}
