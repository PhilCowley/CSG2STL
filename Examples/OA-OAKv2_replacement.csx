Box b1 = new Box(new Vector(-30, -30, 0), new Vector(35, 30, 2));
Box b2 = new Box(new Vector(35, -25, 0), new Vector(45, 25, 2));

Cylinder c1 = new Cylinder(new Vector(45, -30, 1.5), new Vector(45, 30, 1.5), 1.5);

Cylinder c2 = new Cylinder(new Vector(0, 0, 1), new Vector(0, 0, 7.5), 25.1, 60);
Mesh c3 = new Cylinder(new Vector(0, 0, -1), new Vector(0, 0, 8), 23.25, 60).Perturb();

Mesh m = b1.Union(b2).Union(c1).Union(c2).Difference(c3);

//m=m.Difference(new Plane(new Vector(0, 0, 0), new Vector(1, 0, 0)).Perturb());

Mesh t = new Text("D:\\Projects\\CSG2STL\\Examples\\Square721.ttf", "CSG2STL", depth: 10, size: 4);
t = t.Rotate(new Vector(0, 0, -90));
t = t.Translate(new Vector(32, -(t.Min().Y + t.Max().Y) / 2, -1));
m = m.Difference(t);

m.Union(new Cone(new Vector(0, 0, 0), new Vector(10, 0, 0), 5));

m.ExportSTL("D:\\Projects\\CSG2STL\\Examples\\OA-OAKv2_replacement.stl");

