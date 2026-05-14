# CSG2STL
CSG2STL is a command-line tool that converts Constructive Solid Geometry (CSG) files into STL (Stereolithography) 
format, which is widely used for 3D printing and computer-aided design (CAD).

## About
CSG2STL grew out of a wish to use the excellent POVRay softare to create models for 3D printing. However, POVRay's 
CSG capabilities are limited and not suitable for complex models. CSG2STL allows users to define their models using 
a more flexible CSG library in C#, and then export them as STL files for 3D printing.

As a software developer, my mind works in code and I've always struggled with visual modeling tools. CSG2STL allows 
me to create complex 3D models using code, which is more intuitive for me. I hope it can be useful for others who 
prefer a code-based approach to 3D modeling.

Using C# as a scripting language allows for powerful and flexible model definitions, leveraging the full capabilities
of the language as well as the CSG library. This approach is particularly beneficial for users who are comfortable with 
programming and want to create complex models that may be difficult to achieve with traditional visual modeling tools.

## How to use

1. Create an C# script file (e.g., `script.csx`) that defines your CSG model using the CSG library. For example:
 
```
Cylinder c1 = new Cylinder(new Vector(0, 0, 1), new Vector(0, 0, 1), 25);
Cylinder c2 = new Cylinder(new Vector(0, 0, -2), new Vector(0, 0, 2), 20);

Mesh m = c2.Difference(c2.Perturb());

m.ExportSTL("output.stl");
```

2. Run the CSG2STL tool from the command line, providing the path to your C# script: For example:
```	
CSG2STL script.csx
```

3. The tool will execute the script, generate the CSG model, and export it as an STL file (e.g., `output.stl`).

## Objects
CSG2STL supports various geometric primitives. Each is a special case of a general mesh object:
- **Cube**\
		Creates a simple box by definihng two opposite corners.\
		For example:
		```Mesh b = new Box(new Vector(0, 0, 0), new Vector(1, 1, 1));```
- **Sphere**\
		Creates a sphere by defining its center and radius.\
		For example:
		```Mesh s = new Sphere(new Vector(0, 0, 0), 1);```
- **Cylinder**\
		Creates a cylinder by defining its base center, height, and radius.\
		For example:
		```Mesh c = new Cylinder(new Vector(0, 0, 0), new Vector(0, 0, 1), 1);```
- **Cone**\
		Creates a cone by defining its base center, height, and radius.\
		For example:
		```Mesh c = new Cone(new Vector(0, 0, 0), new Vector(0, 0, 1), 1);```
- **Frustum** (truncated cones)\
		Creates a frustum by defining its base center, base radius, top center, and top radius.\
		For example:
		```Mesh f = new Frustum(new Vector(0, 0, 0), 1, new Vector(0, 0, 1), 0.5);```
- **Plane**\
		Creates a plane by defining a point on the plane and the normal at that point.\
		For example:
		```Mesh p = new Plane(new Vector(), new Vector(0, 1, 0));```

## CSG Operations
- **Union**\
		Creates a new mesh that is the union of two meshes.
		\For example:
		```Mesh u = m1.Union(m2);```
- **Difference**\
		Creates a new mesh that is the difference of two meshes (i.e., the parts of the first mesh that are not in the second).\
		For example:
		```Mesh d = m1.Difference(m2);```
- **Intersection**\
		Creates a new mesh that is the intersection of two meshes (i.e., the parts that are in both meshes).\
		For example:
		```Mesh i = m1.Intersection(m2);```

## Modifiers
- **Scale**\
		Scales a mesh by a given factor.\
		For example:
		```Mesh s = m.Scale(2);``` to scale uniformly in all three directions or ```Mesh s = m.Scale(new Vector(2, 1, 1));``` to scale 
		differently in x, y and z.\
		Note that scaling is applied from the origin, so if an object is not centered at the origin, it will be scaled away from the origin. 
		To scale an object around its center, you can first translate it to the origin, scale it, and then translate it back to its original 
		position.
- **Rotate**\
		Rotates a mesh by a given angle around a specified axis. For example:
		```Mesh r = m.Rotate(new Vector(0, 90, 0));```
		Note that rotation is applied around the origin, so if an object is not centered at the origin, it will be rotated around the origin.
- **Translate**\
		Translates a mesh by a given vector. For example:
		```Mesh t = m.Translate(new Vector(1, 0, 0));```
- **Perturb**\
		Adds randomnoise to the mesh to avoid coincident surfaces.\
		For example:
		```Mesh p = m.Perturb();``` or ```Mesh p = m.Perturb(0.1);```
		The amount of perturbation can be controlled by passing a parameter to the Perturb method, which specifies the maximum distance that 
		vertices can be moved. By default the amount of perturbation is 0.0000001, which is small enough to avoid noticeable changes to the 
		model while still preventing coincident surfaces.

## Other classes
- **Vector**\
		Represents a 3D vector for defining points and directions in space. It is used as a fundamental building block for defining the geometry 
		of the CSG models, such as the positions of vertices, the directions of axes, and the parameters for transformations.\
		As well as being a parameter for the geometric primitives and transformations, the Vector class also provides various 
		utility methods for vector operations, such as addition, subtraction, scaling, and normalization. It also provides functions for calculating
		vector and scalar producsts, as well as methods for rotating vectors around different axes. 
- **Mesh**\	
	  Represents a 3D mesh object that can be manipulated using CSG operations and modifiers.\
		The base c;lass for all the primitives and the result of CSG operations is the Mesh class, which provides a common interface for working 
		with the geometry of the models.
- **Facet**\
		Represents a single triangular facet of a mesh, defined by three vertices and a normal vector.\
		While it is used extensively within the code, it is not generally used directly by users of the CSG2STL tool. Instead, users typically 
		work with the geometric primitives and CSG operations to create their models, and the Mesh class is used internally to represent the 
		resulting geometry.

## Exporting
CSG2STL can export the generated CSG model as an STL file, which is widely used for 3D printing and computer-aided design (CAD).

## License
CSG2STL is licensed under the GNU General Public License, Version 3.0 (GNU GPLv3). See the LICENSE file for more information.
