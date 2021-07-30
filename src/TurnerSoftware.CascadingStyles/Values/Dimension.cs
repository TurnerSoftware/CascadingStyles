namespace TurnerSoftware.CascadingStyles.Values
{
	public struct Dimension
	{
		public readonly decimal Number;
		public readonly DimensionUnit Unit;

		public Dimension(decimal number, DimensionUnit unit)
		{
			Number = number;
			Unit = unit;
		}

		public override string ToString()
		{
			return Unit.ToString(Number);
		}

		public static implicit operator string(Dimension value) => value.ToString();
	}
}
