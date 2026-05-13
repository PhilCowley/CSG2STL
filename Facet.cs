using System;
using System.Collections.Generic;
using System.Text;

namespace CSG2STL
{
	public class Facet
	{
		public Vector A { get; set; }
		public Vector B { get; set; }
		public Vector C { get; set; }

		public Facet(Vector a, Vector b, Vector c)
		{
			A = a;
			B = b;
			C = c;
		}

		public Vector Normal
		{
			get
			{
				Vector ab = B - A;
				Vector ac = C - A;
				return ab.Cross(ac).Normalise();
			}
		}

		public Facet Flipped => new Facet(A, C, B);
	}
}
