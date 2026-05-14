using System;

namespace CSG2STL
{
	// Represents a half-space: all points on the solid side of an infinite plane.
	// Approximated as a large box extending 'size' units in every direction along
	// the plane and 'size' units into the solid (away from the normal).
	// Use with Difference to slice a mesh, or with Intersection to clip it.
	public class Plane : Mesh
	{
		public Plane(Vector point, Vector normal, double size = 1000)
		{
			normal = normal.Normalise();

			// Build an orthonormal basis in the plane
			Vector arbitrary = Math.Abs(normal.X) > Math.Abs(normal.Z)
				? new Vector(-normal.Y, normal.X, 0)
				: new Vector(0, -normal.Z, normal.Y);
			Vector u = arbitrary.Normalise();
			Vector v = normal.Cross(u).Normalise();

			// Four corners on the plane surface
			Vector p0 = point - u * size - v * size;
			Vector p1 = point + u * size - v * size;
			Vector p2 = point + u * size + v * size;
			Vector p3 = point - u * size + v * size;

			// Four corners on the far (solid) side
			Vector p4 = p0 - normal * size;
			Vector p5 = p1 - normal * size;
			Vector p6 = p2 - normal * size;
			Vector p7 = p3 - normal * size;

			// Plane surface face — outward normal points in +normal direction
			Facets.Add(new Facet(p0, p1, p3));
			Facets.Add(new Facet(p1, p2, p3));

			// Back face — outward normal points in -normal direction
			Facets.Add(new Facet(p4, p7, p5));
			Facets.Add(new Facet(p5, p7, p6));

			// -v side
			Facets.Add(new Facet(p0, p4, p5));
			Facets.Add(new Facet(p0, p5, p1));

			// +u side
			Facets.Add(new Facet(p1, p5, p6));
			Facets.Add(new Facet(p1, p6, p2));

			// +v side
			Facets.Add(new Facet(p2, p6, p7));
			Facets.Add(new Facet(p2, p7, p3));

			// -u side
			Facets.Add(new Facet(p3, p7, p4));
			Facets.Add(new Facet(p3, p4, p0));
		}
	}
}
