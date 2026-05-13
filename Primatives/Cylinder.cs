using System;

namespace CSG2STL
{
	public class Cylinder : Mesh
	{
		public Cylinder(Vector start, Vector end, double radius, int segments = 32)
		{
			Console.WriteLine("Cylinder(" + start.ToString(), ", " + end.ToString() + ", " + radius.ToString() + ", " + segments.ToString() + ")");
			Vector axis = end - start;

			// Build an orthonormal basis perpendicular to the axis
			Vector arbitrary = Math.Abs(axis.X) > Math.Abs(axis.Z)
				? new Vector(-axis.Y, axis.X, 0)
				: new Vector(0, -axis.Z, axis.Y);

			Vector u = arbitrary.Normalise();
			Vector v = axis.Cross(u).Normalise();

			Vector[] ring0 = new Vector[segments];
			Vector[] ring1 = new Vector[segments];

			for (int i = 0; i < segments; i++)
			{
				double angle = 2 * Math.PI * i / segments;
				double cos = Math.Cos(angle) * radius;
				double sin = Math.Sin(angle) * radius;

				ring0[i] = start + u * cos + v * sin;
				ring1[i] = end   + u * cos + v * sin;
			}

			for (int i = 0; i < segments; i++)
			{
				int next = (i + 1) % segments;

				// Side — two triangles per quad
				Facets.Add(new Facet(ring0[i], ring1[i], ring0[next]));
				Facets.Add(new Facet(ring1[i], ring1[next], ring0[next]));

				// End caps
				Facets.Add(new Facet(start, ring0[next], ring0[i]));
				Facets.Add(new Facet(end,   ring1[i],    ring1[next]));
			}
		}
	}
}
