using System;

namespace CSG2STL
{
	public class Frustum : Mesh
	{
		public Frustum(Vector start, double startRadius, Vector end, double endRadius, int segments = 32)
			: this(start, new Vector(startRadius, startRadius, 0), end, new Vector(endRadius, endRadius, 0), segments) { }

		public Frustum(Vector start, Vector startRadius, Vector end, Vector endRadius, int segments = 32)
		{
			Console.WriteLine("Frustum(" + start.ToString() + ", " + startRadius.ToString() + ", " + end.ToString() + ", " + endRadius.ToString() + ", " + segments.ToString() + ")");

			Vector axis = end - start;

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
				double cos = Math.Cos(angle);
				double sin = Math.Sin(angle);

				ring0[i] = start + u * (cos * startRadius.X) + v * (sin * startRadius.Y);
				ring1[i] = end   + u * (cos * endRadius.X)   + v * (sin * endRadius.Y);
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
