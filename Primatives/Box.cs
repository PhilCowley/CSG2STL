using System.Collections;

namespace CSG2STL
{
	public class Box : Mesh
	{
		public Box(Vector min, Vector max)
		{
			Console.WriteLine("Box(" + min.ToString() + ", " + max.ToString() + ")");

			// c[i]: bit0=X, bit1=Y, bit2=Z selects min or max on each axis
			Vector[] c = new Vector[8];
			for (int i = 0; i < 8; i++)
				c[i] = new Vector(
					(i & 1) != 0 ? max.X : min.X,
					(i & 2) != 0 ? max.Y : min.Y,
					(i & 4) != 0 ? max.Z : min.Z);

			// -Z face
			Facets.Add(new Facet(c[0], c[3], c[1]));
			Facets.Add(new Facet(c[0], c[2], c[3]));
			// +Z face
			Facets.Add(new Facet(c[4], c[5], c[7]));
			Facets.Add(new Facet(c[4], c[7], c[6]));
			// -Y face
			Facets.Add(new Facet(c[0], c[1], c[5]));
			Facets.Add(new Facet(c[0], c[5], c[4]));
			// +Y face
			Facets.Add(new Facet(c[2], c[7], c[3]));
			Facets.Add(new Facet(c[2], c[6], c[7]));
			// -X face
			Facets.Add(new Facet(c[0], c[4], c[6]));
			Facets.Add(new Facet(c[0], c[6], c[2]));
			// +X face
			Facets.Add(new Facet(c[1], c[3], c[7]));
			Facets.Add(new Facet(c[1], c[7], c[5]));
		}
	}
}
