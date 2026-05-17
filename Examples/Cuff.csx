Mesh inner = new Frustum(
	start: new Vector(0, 0, -1),
	startRadius: new Vector(30, 21, 0),
	end: new Vector(0, 0, 26),
	endRadius: new Vector(28, 20, 0),
	60);
Mesh outer = new Frustum(
	start: new Vector(0, 0, 0),
	startRadius: new Vector(31, 22, 0),
	end: new Vector(0, 0, 25),
	endRadius: new Vector(29, 21, 0),
	60);

Mesh m = outer.Difference(inner.Perturb(0.1));

m.ExportSTL("D:\\Projects\\CSG2STL\\Examples\\Cuff.stl");