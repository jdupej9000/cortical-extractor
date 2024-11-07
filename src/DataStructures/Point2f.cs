using CorticalExtract.Processing;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorticalExtract.DataStructures
{
    public class Point2f
    {
        public Point2f()
        {
        }

        public Point2f(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

        public Point2f(Point2f other)
        {
            this.x = other.x;
            this.y = other.y;
        }

        float x, y;

        public PointF ToPointF(float s=1)
        {
            return new PointF(x*s, y*s);
        }

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

        public float LengthSq
        {
            get { return Dot(this, this); }
        }

        public float Length
        {
            get { return (float)Math.Sqrt(LengthSq); }
        }
        #endregion

        #region Operators
        public static Point2f operator -(Point2f a, Point2f b)
        {
            return new Point2f(a.x - b.x, a.y - b.y);
        }

        public static Point2f operator +(Point2f a, Point2f b)
        {
            return new Point2f(a.x + b.x, a.y + b.y);
        }

        public static Point2f operator -(Point2f a)
        {
            return new Point2f(-a.x, -a.y);
        }

        public static Point2f operator *(Point2f a, float q)
        {
            return new Point2f(a.x * q, a.y * q);
        }

        public static Point2f operator *(float q, Point2f a)
        {
            return a * q;
        }

        public static Point2f operator *(Point2f a, Point2f b)
        {
            return new Point2f(a.x * b.x, a.y * b.y);
        }

        public static Point2f operator /(Point2f a, Point2f b)
        {
            return new Point2f(a.x / b.x, a.y / b.y);
        }
        #endregion
        
        #region Static
        public static float Dot(Point2f a, Point2f b)
        {
            return (a.x * b.x) + (a.y * b.y);
        }

        public static float DistanceSq(Point2f a, Point2f b)
        {
            return Utils.Sqr(a.x - b.x) + Utils.Sqr(a.y - b.y);
        }

        public static float Distance(Point2f a, Point2f b)
        {
            return (float)Math.Sqrt(Utils.Sqr(a.x - b.x) + Utils.Sqr(a.y - b.y));
        }
        #endregion
    }
}
