using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sandbox
{
	public class Inverview
	{
	}

	public abstract class Coordinate
	{
		public int X { get; set; }
		public int Y { get; set; }
	}
	public class Point : Coordinate
	{
		public Point(int x, int y)
		{
			X = x;
			Y = y;
		}
	}
	public class Rectange : Coordinate
	{
		public int Width { get; set; }
		public int Height { get; set; }
	}
}
