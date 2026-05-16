Mesh m = new Mesh();

for(int sx = -1; sx <= 1; sx += 2)
{
	Mesh c = new Cylinder(new Vector(0, 0, 0), new Vector(0, 0, 2), 6);
	c = c.Difference(new Plane(new Vector(0, 0, 0), new Vector(1, 0, 0)));
	c = c.Translate(new Vector(20, 0, 0));
	c = c.Scale(new Vector(sx, 1, 1));
	m = m.Union(c);
}
m = m.Union(new Box(new Vector(-20.001, -6, 0), new Vector(20.001, 6, 2)));
m = m.Difference(new Cylinder(new Vector(-23, 0, -1), new Vector(-24, 0, 3), 1.5));

Mesh t = new Text("D:\\Projects\\CSG2STL\\Examples\\Square721.ttf", "Phil & Tabby", depth: 1, size: 7);
t = t.Translate(new Vector(-(t.Min().X + t.Max().X) / 2, -(t.Min().Y + t.Max().Y) / 2, 1.99));
t = t.Translate(new Vector(1, 0, 0));
m = m.Union(t);

m.ExportSTL("D:\\Projects\\CSG2STL\\Examples\\KeyFob.stl");