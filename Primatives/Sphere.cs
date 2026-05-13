using System;
using System.Collections;

namespace CSG2STL
{
	internal class Sphere : Mesh
	{
		public Sphere(Vector centre, double radius, int stacks = 16, int slices = 32)
		{
			Console.WriteLine("Sphere(" + centre.ToString(), ", " + radius.ToString() + ", " + stacks.ToString() + ", " + slices.ToString() + ")");

			// UV sphere: stacks rows of quads between latitude rings, plus polar caps.
			Vector[][] rings = new Vector[stacks + 1][];
			for (int i = 0; i <= stacks; i++)
			{
				double phi = Math.PI * i / stacks; // 0 (north pole) → π (south pole)
				rings[i] = new Vector[slices];
				for (int j = 0; j < slices; j++)
				{
					double theta = 2 * Math.PI * j / slices;
					rings[i][j] = centre + new Vector(
						radius * Math.Sin(phi) * Math.Cos(theta),
						radius * Math.Sin(phi) * Math.Sin(theta),
						radius * Math.Cos(phi));
				}
			}

			Vector north = centre + new Vector(0, 0,  radius);
			Vector south = centre + new Vector(0, 0, -radius);

			for (int j = 0; j < slices; j++)
			{
				int next = (j + 1) % slices;

				// North polar cap
				Facets.Add(new Facet(north, rings[1][next], rings[1][j]));

				// South polar cap
				Facets.Add(new Facet(south, rings[stacks - 1][j], rings[stacks - 1][next]));

				// Middle quads
				for (int i = 1; i < stacks - 1; i++)
				{
					Facets.Add(new Facet(rings[i][j],    rings[i + 1][j],    rings[i][next]));
					Facets.Add(new Facet(rings[i][next], rings[i + 1][j],    rings[i + 1][next]));
				}
			}
		}
	}
}
