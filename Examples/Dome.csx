Mesh outer = new Sphere(new Vector(0, 0, 0), 20, 120, 24);
Mesh inner = new Sphere(new Vector(0, 0, 0), 19.5, 120, 24);

Mesh dome = outer.Difference(inner.Perturb()).Difference(new Plane(new Vector(0, 0, 0), new Vector(0, 0, 1)));

Console.WriteLine("Dome=" + dome);

dome.ExportSTL("D:\\Projects\\CSG2STL\\Examples\\Dome.stl"); 
dome.Export3MF("D:\\Projects\\CSG2STL\\Examples\\Dome.3mf");