using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace CorticalExtract.DataStructures
{
    public class Point3f
    {
        public Point3f()
            : this(0, 0, 0)
        {
        }

        public Point3f(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        protected float x, y, z;

        #region Properties
        public float X
        {
            get { return x; }
            set { x = value; }
        }

        public float Y
        {
            get { return y; }
            set { y = value; }
        }

        public float Z
        {
            get { return z; }
            set { z = value; }
        }

        public Vector2 XY
        {
            get { return new Vector2(x, y); }
        }
    

        public float LengthSq
        {
            get { return Dot(this, this); }
        }

        public float Length
        {
            get { return (float)Math.Sqrt(LengthSq); }
        }
        #endregion

        #region Transformations
        public Point3f Normalize()
        {
            var length = Length;

            if (length <= Math.Sqrt(float.MinValue))
                return new Point3f(1, 0, 0);

            X /= length;
            Y /= length;
            Z /= length;

            return this;
        }

        public Point3f Scale(float[] voxelDim)
        {
            x *= voxelDim[0];
            y *= voxelDim[1];
            z *= voxelDim[2];

            return this;
        }

        public Point3f UnScale(float[] voxelDim)
        {
            x /= voxelDim[0];
            y /= voxelDim[1];
            z /= voxelDim[2];

            return this;
        }

        #endregion

        #region Operators
        public static Point3f operator -(Point3f a, Point3f b)
        {
            return new Point3f(a.x - b.x, a.y - b.y, a.z - b.z);
        }

        public static Point3f operator +(Point3f a, Point3f b)
        {
            return new Point3f(a.x + b.x, a.y + b.y, a.z + b.z);
        }

        public static Point3f operator -(Point3f a)
        {
            return new Point3f(-a.x, -a.y, -a.z);
        }

        public static Point3f operator *(Point3f a, float q)
        {
            return new Point3f(a.x * q, a.y * q, a.z * q);
        }

        public static Point3f operator *(float q, Point3f a)
        {
            return a * q;
        }

        public static Point3f operator *(Point3f a, Point3f b)
        {
            return new Point3f(a.x * b.x, a.y * b.y, a.z * b.z);
        }

        public static Point3f operator /(Point3f a, Point3f b)
        {
            return new Point3f(a.x / b.x, a.y / b.y, a.z / b.z);
        }
        #endregion

        #region Static
        public static Point3f Cross(Point3f a, Point3f b)
        {
            return new Point3f((a.y * b.z - a.z * b.y),
                               (a.z * b.x - a.x * b.z),
                               (a.x * b.y - a.y * b.x));
        } 
        
        public static float Dot(Point3f a, Point3f b)
        {
            return (a.x * b.x) + (a.y * b.y) + (a.z * b.z);
        }

        public static float DistanceAniso(Point3f a, Point3f b, float[] voxelDim)
        {
            float dx = voxelDim[0] * (a.x - b.x);
            float dy = voxelDim[1] * (a.y - b.y);
            float dz = voxelDim[2] * (a.z - b.z);

            return (float)Math.Sqrt(dx * dx + dy * dy + dz * dz);
        }

        #endregion
    }
}
