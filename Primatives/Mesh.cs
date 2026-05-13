using System;
using System.Collections.Generic;
using System.IO;

namespace CSG2STL
{
	public class Mesh
	{
		public List<Facet> Facets { get; } = new List<Facet>();

		public void ExportSTL(string filename)
		{
			// Binary STL: 80-byte header, uint32 triangle count,
			// then per triangle: float32 normal (x,y,z), three float32 vertices, uint16 attribute.
			using BinaryWriter w = new BinaryWriter(File.Open(filename, FileMode.Create));

			byte[] header = new byte[80];
			w.Write(header);
			w.Write((uint)Facets.Count);

			foreach (Facet f in Facets)
			{
				Vector n = f.Normal;
				w.Write((float)n.X); w.Write((float)n.Y); w.Write((float)n.Z);

				w.Write((float)f.A.X); w.Write((float)f.A.Y); w.Write((float)f.A.Z);
				w.Write((float)f.B.X); w.Write((float)f.B.Y); w.Write((float)f.B.Z);
				w.Write((float)f.C.X); w.Write((float)f.C.Y); w.Write((float)f.C.Z);

				w.Write((ushort)0);
			}
		}

		public Mesh Perturb()
		{
			const double amount = 0.0000001;
			Random rng = new Random();
			Vector PerturbVertex(Vector p) => new Vector(
				p.X + (rng.NextDouble() * 2 - 1) * amount,
				p.Y + (rng.NextDouble() * 2 - 1) * amount,
				p.Z + (rng.NextDouble() * 2 - 1) * amount);

			Mesh result = new Mesh();
			foreach (Facet f in Facets)
				result.Facets.Add(new Facet(PerturbVertex(f.A), PerturbVertex(f.B), PerturbVertex(f.C)));
			return result;
		}

		public Mesh Translate(Vector offset)
		{
			Mesh result = new Mesh();
			foreach (Facet f in Facets)
				result.Facets.Add(new Facet(f.A + offset, f.B + offset, f.C + offset));
			return result;
		}

		public Mesh Rotate(Vector r)
		{
			// Apply rotations in X → Y → Z order (angles in degrees).
			const double deg2rad = Math.PI / 180.0;
			double cx = Math.Cos(r.X * deg2rad), sx = Math.Sin(r.X * deg2rad);
			double cy = Math.Cos(r.Y * deg2rad), sy = Math.Sin(r.Y * deg2rad);
			double cz = Math.Cos(r.Z * deg2rad), sz = Math.Sin(r.Z * deg2rad);

			Vector RotateVertex(Vector p)
			{
				// X axis
				double y1 = p.Y * cx - p.Z * sx;
				double z1 = p.Y * sx + p.Z * cx;
				// Y axis
				double x2 = p.X * cy + z1 * sy;
				double z2 = -p.X * sy + z1 * cy;
				// Z axis
				double x3 = x2 * cz - y1 * sz;
				double y3 = x2 * sz + y1 * cz;
				return new Vector(x3, y3, z2);
			}

			Mesh result = new Mesh();
			foreach (Facet f in Facets)
				result.Facets.Add(new Facet(RotateVertex(f.A), RotateVertex(f.B), RotateVertex(f.C)));
			return result;
		}

		public Mesh Scale(double factor) => Scale(new Vector(factor, factor, factor));

		public Mesh Scale(Vector factor)
		{
			Mesh result = new Mesh();
			foreach (Facet f in Facets)
				result.Facets.Add(new Facet(
					new Vector(f.A.X * factor.X, f.A.Y * factor.Y, f.A.Z * factor.Z),
					new Vector(f.B.X * factor.X, f.B.Y * factor.Y, f.B.Z * factor.Z),
					new Vector(f.C.X * factor.X, f.C.Y * factor.Y, f.C.Z * factor.Z)));
			return result;
		}

		public Mesh Difference(Mesh other)
		{
			Console.Write("Difference() .");

			List<Facet> splitA = SplitAgainst(this.Facets, other.Facets);
			List<Facet> splitB = SplitAgainst(other.Facets, this.Facets);

			Mesh result = new Mesh();

			result.Facets.AddRange(splitA.AsParallel().Where(f => !other.Contains(Centroid(f))));

			// B's faces that are inside A become interior walls — flip their normals
			result.Facets.AddRange(splitB.AsParallel().Where(f => this.Contains(Centroid(f))).Select(f => f.Flipped));

			Console.WriteLine();
			return result;
		}

		public Mesh Intersection(Mesh other)
		{
			Console.Write("Intersection() .");

			List<Facet> splitA = SplitAgainst(this.Facets, other.Facets);
			List<Facet> splitB = SplitAgainst(other.Facets, this.Facets);

			Mesh result = new Mesh();

			result.Facets.AddRange(splitA.AsParallel().Where(f => other.Contains(Centroid(f))));
			result.Facets.AddRange(splitB.AsParallel().Where(f => this.Contains(Centroid(f))));

			Console.WriteLine();
			return result;
		}

		public Mesh Union(Mesh other)
		{
			Console.Write("Union() .");

			List<Facet> splitA = SplitAgainst(this.Facets, other.Facets);
			List<Facet> splitB = SplitAgainst(other.Facets, this.Facets);

			Mesh result = new Mesh();

			result.Facets.AddRange(splitA.AsParallel().Where(f => !other.Contains(Centroid(f))));
			result.Facets.AddRange(splitB.AsParallel().Where(f => !this.Contains(Centroid(f))));

			Console.WriteLine();
			return result;
		}

		// ── Point-in-mesh (ray cast) ───────────────────────────────────────────────

		// Lazily computed and cached — valid for the lifetime of this Mesh instance.
		private (Vector min, Vector max)? _aabb;
		private (Vector min, Vector max) GetAABB()
		{
			if (_aabb == null)
			{
				double minX = double.MaxValue, minY = double.MaxValue, minZ = double.MaxValue;
				double maxX = double.MinValue, maxY = double.MinValue, maxZ = double.MinValue;
				foreach (Facet f in Facets)
				{
					foreach (Vector v in new[] { f.A, f.B, f.C })
					{
						if (v.X < minX) minX = v.X; if (v.X > maxX) maxX = v.X;
						if (v.Y < minY) minY = v.Y; if (v.Y > maxY) maxY = v.Y;
						if (v.Z < minZ) minZ = v.Z; if (v.Z > maxZ) maxZ = v.Z;
					}
				}
				_aabb = (new Vector(minX, minY, minZ), new Vector(maxX, maxY, maxZ));
			}
			return _aabb.Value;
		}

		private bool Contains(Vector point)
		{
			// Fast AABB pre-reject — if the point is outside the bounding box it
			// cannot be inside the mesh, so skip all three ray casts entirely.
			var (min, max) = GetAABB();
			if (point.X < min.X || point.X > max.X ||
			    point.Y < min.Y || point.Y > max.Y ||
			    point.Z < min.Z || point.Z > max.Z) return false;

			// Three off-axis rays; majority vote guards against rays that graze
			// through edges or vertices, which flip parity and give wrong results.
			int votes = 0;
			if (CountRayHits(point, new Vector(1.00000, 0.00031, 0.00053)) % 2 == 1) votes++;
			if (CountRayHits(point, new Vector(0.00031, 1.00000, 0.00071)) % 2 == 1) votes++;
			if (CountRayHits(point, new Vector(0.00053, 0.00071, 1.00000)) % 2 == 1) votes++;
			return votes >= 2;
		}

		private int CountRayHits(Vector origin, Vector dir)
		{
			int count = 0;
			foreach (Facet f in Facets)
				if (RayIntersectsTriangle(origin, dir, f))
					count++;
			return count;
		}

		private static Vector Centroid(Facet f) => (f.A + f.B + f.C) / 3.0;

		private static bool RayIntersectsTriangle(Vector origin, Vector dir, Facet tri)
		{
			const double eps = 1e-8;
			Vector edge1 = tri.B - tri.A;
			Vector edge2 = tri.C - tri.A;
			Vector h = dir.Cross(edge2);
			double a = edge1.Dot(h);
			if (Math.Abs(a) < eps) return false;
			double f = 1.0 / a;
			Vector s = origin - tri.A;
			double u = f * s.Dot(h);
			if (u < 0 || u > 1) return false;
			Vector q = s.Cross(edge1);
			double v = f * dir.Dot(q);
			if (v < 0 || u + v > 1) return false;
			return f * edge2.Dot(q) > eps;
		}

		// ── Splitting ──────────────────────────────────────────────────────────────

		private static (Vector min, Vector max) FacetAABB(Facet f) => (
			new Vector(Math.Min(f.A.X, Math.Min(f.B.X, f.C.X)),
			           Math.Min(f.A.Y, Math.Min(f.B.Y, f.C.Y)),
			           Math.Min(f.A.Z, Math.Min(f.B.Z, f.C.Z))),
			new Vector(Math.Max(f.A.X, Math.Max(f.B.X, f.C.X)),
			           Math.Max(f.A.Y, Math.Max(f.B.Y, f.C.Y)),
			           Math.Max(f.A.Z, Math.Max(f.B.Z, f.C.Z))));

		private static bool AABBOverlap(
			(Vector min, Vector max) a,
			(Vector min, Vector max) b) =>
				a.min.X <= b.max.X && a.max.X >= b.min.X &&
				a.min.Y <= b.max.Y && a.max.Y >= b.min.Y &&
				a.min.Z <= b.max.Z && a.max.Z >= b.min.Z;

		// Splits every triangle in toSplit that intersects any triangle in cutters.
		private static List<Facet> SplitAgainst(List<Facet> toSplit, List<Facet> cutters)
		{
			// Precompute cutter AABBs once — reused for every triangle in the inner loop.
			var cutterBoxes = new (Vector min, Vector max)[cutters.Count];
			for (int i = 0; i < cutters.Count; i++)
				cutterBoxes[i] = FacetAABB(cutters[i]);

			List<Facet> result = new List<Facet>(toSplit);
			for (int ci = 0; ci < cutters.Count; ci++)
			{
				Facet cutter = cutters[ci];
				var cbox = cutterBoxes[ci];
				List<Facet> next = new List<Facet>(result.Count);
				foreach (Facet tri in result)
				{
					if (AABBOverlap(FacetAABB(tri), cbox))
						next.AddRange(SplitByFacet(tri, cutter));
					else
						next.Add(tri);
				}
				result = next;
				UpdateProgress();
			}
			return result;
		}

		// Splits tri along cutter's plane if tri straddles it, returning 1 or 3 sub-triangles.
		// Splitting by the full plane (not just the cutter triangle's extent) ensures every
		// sub-triangle is entirely on one side of the plane, so Contains() always gets a
		// centroid that is clearly inside or outside — not sitting on the boundary.
		private static List<Facet> SplitByFacet(Facet tri, Facet cutter)
		{
			const double eps = 1e-8;
			var single = new List<Facet> { tri };

			// Signed distances of tri's vertices to cutter's plane
			Vector cn = (cutter.B - cutter.A).Cross(cutter.C - cutter.A);
			double cd = cn.Dot(cutter.A);
			Vector[] tv = { tri.A, tri.B, tri.C };
			double[] d = { cn.Dot(tv[0]) - cd, cn.Dot(tv[1]) - cd, cn.Dot(tv[2]) - cd };
			int[]    s = { d[0] > eps ? 1 : d[0] < -eps ? -1 : 0,
			               d[1] > eps ? 1 : d[1] < -eps ? -1 : 0,
			               d[2] > eps ? 1 : d[2] < -eps ? -1 : 0 };

			// All vertices on the same side (or on the plane) → nothing to split
			if (s[0] >= 0 && s[1] >= 0 && s[2] >= 0) return single;
			if (s[0] <= 0 && s[1] <= 0 && s[2] <= 0) return single;

			// Find the two edges that cross strictly from one side to the other
			var cross = new List<(Vector p, int e)>(2);
			for (int i = 0; i < 3; i++)
			{
				int j = (i + 1) % 3;
				if (s[i] * s[j] < 0) // strictly opposite sides
				{
					double t = d[i] / (d[i] - d[j]);
					cross.Add((tv[i] + (tv[j] - tv[i]) * t, i));
				}
			}

			if (cross.Count != 2) return single;

			return SplitTriangle(tri, cross[0].p, cross[0].e, cross[1].p, cross[1].e);
		}

		// Splits tri into 3 sub-triangles along the segment p0→p1,
		// where p0 lies on edge pe and p1 lies on edge qe.
		//
		// The cut isolates whichever vertex is shared by edges pe and qe.
		// All three output triangles inherit the original CCW winding.
		//
		// Canonical forward-adjacent pairs: (0,1) (1,2) (2,0).
		// Reverse pairs are normalised by swapping p0/p1 before processing.
		private static List<Facet> SplitTriangle(Facet tri, Vector p0, int pe, Vector p1, int qe)
		{
			// Normalise reverse-adjacent pairs → canonical
			if ((pe == 1 && qe == 0) || (pe == 2 && qe == 1) || (pe == 0 && qe == 2))
			{
				(p0, p1) = (p1, p0);
				(pe, qe) = (qe, pe);
			}

			// iso  = the isolated vertex (shared by the two cut edges)
			// Formula holds for all three canonical cases:
			//   (0,1)→iso=1  (1,2)→iso=2  (2,0)→iso=0
			Vector[] v  = { tri.A, tri.B, tri.C };
			int     iso = (pe + 1) % 3;

			return new List<Facet>
			{
				// Triangle containing the isolated vertex
				new Facet(p0, v[iso], p1),
				// Two triangles filling the opposite quad
				new Facet(v[(iso + 2) % 3], p0, p1),
				new Facet(v[(iso + 2) % 3], p1, v[(iso + 1) % 3])
			};
		}

		private static int progressCount=0;
		private static void UpdateProgress()
		{
			if(progressCount++ > 1000)
			{
				Console.Write(".");
				progressCount = 0;
			}
		}
	}
}
