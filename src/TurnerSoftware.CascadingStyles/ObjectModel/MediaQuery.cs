using System.Collections.Generic;

namespace TurnerSoftware.CascadingStyles.ObjectModel
{
	public class MediaQuery : ICssRule
	{
		public List<MediaQueryGroup> Query { get; init; } = new List<MediaQueryGroup>();

		public List<ICssRule> Rules { get; init; } = new List<ICssRule>();
	}

	public struct MediaQueryGroup
	{
		public readonly IReadOnlyList<IMediaQueryComponent> Components;

		public MediaQueryGroup(IReadOnlyList<IMediaQueryComponent> components)
		{
			Components = components;
		}
	}

	public interface IMediaQueryComponent { }

	public sealed class MediaExpression : IMediaQueryComponent
	{
		public readonly StringValue Feature;
		public readonly StringValue Expression;
	}

	public sealed class MediaQueryConstants : IMediaQueryComponent
	{
		public static readonly MediaQueryConstants Only = new("ONLY");
		public static readonly MediaQueryConstants Not = new("NOT");
		public static readonly MediaQueryConstants And = new("AND");

		private readonly string Value;

		private MediaQueryConstants(string value)
		{
			Value = value;
		}

		public override string ToString() => Value;
	}
}
