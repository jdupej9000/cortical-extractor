using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CorticalExtract.DataStructures
{
    public static class Extends
    {
        public static PointF ToPointF(this Vector2 v)
        {
            return new PointF(v.X, v.Y);
        }

        public static PointF ToPointF(this Vector2 v, float scale)
        {
            return new PointF(scale * v.X, scale * v.Y);
        }
    }
}
