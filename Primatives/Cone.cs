using System;
using System.Collections;

namespace CSG2STL
{
	public class Cone : Mesh
	{
		public Cone(Vector apex, Vector baseCenter, double radius, int segments = 32)
		{
			Console.WriteLine("Cone(" + apex.ToString(), ", " + baseCenter.ToString() + ", " + radius.ToString() + ", " + segments.ToString() + ")");

			Vector axis = baseCenter - apex;

			Vector arbitrary = Math.Abs(axis.X) > Math.Abs(axis.Z)
				? new Vector(-axis.Y, axis.X, 0)
				: new Vector(0, -axis.Z, axis.Y);

			Vector u = arbitrary.Normalise();
			Vector v = axis.Cross(u).Normalise();

			Vector[] ring = new Vector[segments];
			for (int i = 0; i < segments; i++)
			{
				double angle = 2 * Math.PI * i / segments;
				ring[i] = baseCenter + u * (Math.Cos(angle) * radius)
				                     + v * (Math.Sin(angle) * radius);
			}

			for (int i = 0; i < segments; i++)
			{
				int next = (i + 1) % segments;

				// Side
				Facets.Add(new Facet(apex, ring[i], ring[next]));

				// Base cap
				Facets.Add(new Facet(baseCenter, ring[next], ring[i]));
			}
		}
	}
}
