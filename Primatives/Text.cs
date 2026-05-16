using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.Versioning;
using LibTessDotNet;

namespace CSG2STL
{
	[SupportedOSPlatform("windows")]
	public class Text : Mesh
	{
		public Text(string fontPath, string text, double depth, double size = 72)
		{
			Console.WriteLine($"Text(\"{text}\", fontPath={fontPath}, depth={depth}, size={size})");

			using var fonts = new System.Drawing.Text.PrivateFontCollection();
			fonts.AddFontFile(fontPath);
			var family = fonts.Families[0];

			using var path = new GraphicsPath();
			path.AddString(text, family, (int)FontStyle.Regular, (float)size,
				new PointF(0, 0), StringFormat.GenericTypographic);
			path.Flatten(null, 0.25f);

			var contours = ExtractContours(path);
			BuildMesh(contours, depth);
		}

		private static List<List<PointF>> ExtractContours(GraphicsPath path)
		{
			var contours = new List<List<PointF>>();
			var current = new List<PointF>();
			var pts = path.PathPoints;
			var types = path.PathTypes;

			for (int i = 0; i < pts.Length; i++)
			{
				byte baseType = (byte)(types[i] & 0x07);
				bool close = (types[i] & 0x80) != 0;

				if (baseType == 0 && current.Count > 0)
				{
					contours.Add(current);
					current = new List<PointF>();
				}

				current.Add(pts[i]);

				if (close && current.Count > 0)
				{
					contours.Add(current);
					current = new List<PointF>();
				}
			}

			if (current.Count > 0)
				contours.Add(current);

			return contours;
		}

		private void BuildMesh(List<List<PointF>> contours, double depth)
		{
			// Tessellate the 2D outlines; EvenOdd winding handles letter holes (O, B, D…)
			var tess = new Tess();
			foreach (var c in contours)
			{
				var verts = new ContourVertex[c.Count];
				for (int i = 0; i < c.Count; i++)
					verts[i] = new ContourVertex { Position = new Vec3 { X = c[i].X, Y = -c[i].Y, Z = 0 } };
				tess.AddContour(verts);
			}
			tess.Tessellate(WindingRule.EvenOdd, ElementType.Polygons, 3);

			// Front (z=depth) and back (z=0) faces
			for (int i = 0; i < tess.ElementCount; i++)
			{
				var p0 = tess.Vertices[tess.Elements[i * 3]].Position;
				var p1 = tess.Vertices[tess.Elements[i * 3 + 1]].Position;
				var p2 = tess.Vertices[tess.Elements[i * 3 + 2]].Position;

				// Front face — CCW from +z, normal points to +z
				Facets.Add(new Facet(
					new Vector(p0.X, p0.Y, depth),
					new Vector(p1.X, p1.Y, depth),
					new Vector(p2.X, p2.Y, depth)));

				// Back face — reversed winding, normal points to -z
				Facets.Add(new Facet(
					new Vector(p0.X, p0.Y, 0),
					new Vector(p2.X, p2.Y, 0),
					new Vector(p1.X, p1.Y, 0)));
			}

			// Side walls — one quad (two triangles) per edge
			foreach (var c in contours)
			{
				// Signed area in Y-flipped space: >0 = CCW = outer contour, <0 = CW = hole
				double area = 0;
				for (int i = 0; i < c.Count; i++)
				{
					int j = (i + 1) % c.Count;
					area += (double)c[i].X * (-c[j].Y) - (double)c[j].X * (-c[i].Y);
				}
				bool isOuter = area > 0;

				for (int i = 0; i < c.Count; i++)
				{
					int j = (i + 1) % c.Count;
					var vA = new Vector(c[i].X, -c[i].Y, 0);
					var vB = new Vector(c[j].X, -c[j].Y, 0);
					var vC = new Vector(c[j].X, -c[j].Y, depth);
					var vD = new Vector(c[i].X, -c[i].Y, depth);

					// Winding chosen so the normal points away from the letter's solid interior
					if (isOuter)
					{
						Facets.Add(new Facet(vA, vB, vC));
						Facets.Add(new Facet(vA, vC, vD));
					}
					else
					{
						Facets.Add(new Facet(vA, vC, vB));
						Facets.Add(new Facet(vA, vD, vC));
					}
				}
			}
		}
	}
}
