using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TurnerSoftware.CascadingStyles.ObjectModel
{
	public struct Dimension
	{
		public readonly double Number;
		public readonly StringValue Unit;

		public Dimension(double number, StringValue unit)
		{
			Number = number;
			Unit = unit;
		}
	}
}
