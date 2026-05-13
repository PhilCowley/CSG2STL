using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSG2STL
{
	public class Vector
	{
		public double X { get; set; }
		public double Y { get; set; }
		public double Z { get; set; }

		public Vector(double x, double y)
		{
			X = x;
			Y = y;
			Z = 0;
		}
		public Vector(double x, double y, double z)
		{
			X = x;
			Y = y;
			Z = z;
		}
		public Vector(Vector vOther)
		{
			X = vOther.X;
			Y = vOther.Y;
			Z = vOther.Z;
		}

		public override string ToString()
		{
			return "<" + X.ToString() + ", " + Y.ToString() + ", " + Z.ToString() + ">";
		}

		public static Vector operator +(Vector a, Vector b)
		{
			return new Vector(a.X+b.X, a.Y+b.Y, a.Z+b.Z);
		}
		public static Vector operator -(Vector a, Vector b)
		{
			return new Vector(a.X-b.X, a.Y-b.Y, a.Z-b.Z);
		}
		public static Vector operator +(Vector a)
		{
			return a;
		}
		public static Vector operator -(Vector a)
		{
			return new Vector(-a.X, -a.Y, -a.Z);
		}

		public static Vector operator *(Vector a, Vector b)
		{
			return new Vector(a.X * b.X, a.Y * b.Y, a.Z * b.Z);
		}
		public static Vector operator *(Vector a, double dScalar)
		{
			return new Vector(a.X * dScalar, a.Y * dScalar, a.Z * dScalar);
		}
		public static Vector operator /(Vector a, Vector b)
		{
			return new Vector(a.X / b.X, a.Y / b.Y, a.Z / b.Z);
		}
		public static Vector operator /(Vector a, double scalar)
		{
			return new Vector(a.X / scalar, a.Y / scalar, a.Z / scalar);
		}

		//public static bool operator true(Vector a)
		//{
		//	if(a.X == 0 && a.Y == 0 && a.Z == 0)
		//		return true;
		//	return false;
		//}
		//public static bool operator false(Vector a)
		//{
		//	if(a.X == 0 && a.Y == 0 && a.Z == 0)
		//		return false;
		//	return true;
		//}

		public double Length
		{
			get{ return Math.Sqrt((X*X)+(Y*Y)+(Z*Z)); }
			set{ Normalise(value); }
		}

		public Vector Normalise(double dNewLength = 1)
		{
			double sScaleFactor = Length/dNewLength;
			return new Vector	(X/sScaleFactor, Y/sScaleFactor, Z/sScaleFactor);
		}

		public Vector Cross(Vector vOther)
		{
			return new Vector((Y*vOther.Z)-(Z*vOther.Y), (Z*vOther.X)-(X*vOther.Z), (X*vOther.Y)-(Y*vOther.X));
		}

		public double Dot(Vector vOther)
		{
			return (X*vOther.X)+(Y*vOther.Y)+(Z*vOther.Z);
		}

		static public Vector Perpendicular(Vector a, Vector b)
		{ 
			return a.Cross(b).Normalise();
		}
	}
}
