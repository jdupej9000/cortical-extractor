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
    public static class VectorUtils
    {
        public static PointF ToPointF(this Vector2 v)
        {
            return new PointF(v.X, v.Y);
        }

        public static PointF ToPointF(this Vector2 v, float scale)
        {
            return new PointF(scale * v.X, scale * v.Y);
        }

        public static Vector2 XY(this Vector3 v)
        {
            return new Vector2(v.X, v.Y);
        }

        public static float DistanceAniso(Vector3 a, Vector3 b, float[] voxelDim)
        {
            return ((a - b) * new Vector3(voxelDim.AsSpan())).Length();

            /*float dx = voxelDim[0] * (a.X - b.x);
            float dy = voxelDim[1] * (a.y - b.y);
            float dz = voxelDim[2] * (a.z - b.z);

            return (float)Math.Sqrt(dx * dx + dy * dy + dz * dz);*/
        }
    }
}
