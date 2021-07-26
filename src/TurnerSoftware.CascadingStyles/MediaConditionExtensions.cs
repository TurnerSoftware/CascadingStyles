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
		public static MediaCondition MinWidth(this MediaCondition condition, string value) => condition.Feature("min-width", value);
	}
}
