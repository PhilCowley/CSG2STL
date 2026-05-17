Mesh m = new Mesh();

//Text
Mesh t = new Text("D:\\Projects\\CSG2STL\\Examples\\B5 Regular.ttf", "Henry", depth: 2, size: 12);
t = t.Translate(new Vector(-(t.Min().X + t.Max().X) / 2, -(t.Min().Y + t.Max().Y) / 2, 1));
t = t.Translate(new Vector(1, 0, 0));

double textLength = t.Max().X - t.Min().X;

for(int sx = -1; sx <= 1; sx += 2)
{
	Mesh c = new Cylinder(new Vector(0, 0, 0), new Vector(0, 0, 2), 8);
	c = c.Difference(new Plane(new Vector(0, 0, 0), new Vector(1, 0, 0)));
	c = c.Translate(new Vector(textLength/2, 0, 0));
	c = c.Scale(new Vector(sx, 1, 1));
	m = m.Union(c);
}
m = m.Union(new Box(new Vector(-textLength/2 - 0.001, -8, 0), new Vector(textLength/2 + 0.001, 8, 2)));

//Hole for keyring
m = m.Difference(new Cylinder(new Vector(0, 0, -1), new Vector(0, 0, 3), 2.5).Translate(new Vector(-textLength / 2 - 3, 0, 0)));

m = m.Difference(t);

m.ExportSTL("D:\\Projects\\CSG2STL\\Examples\\KeyFob.stl");