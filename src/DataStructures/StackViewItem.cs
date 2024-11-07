using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace CorticalExtract.DataStructures
{
    public class StackViewItem
    {
        private StackViewItem()
        {
            center = new Point3f[] { new Point3f(0, 0, 0) };
            t0 = new Point3f(1, 0, 0);
            t1 = new Point3f(0, 1, 0);
            voxDim = new float[3] { 1, 1, 1 };
        }

        public StackViewItem(string name, ImageStack stk)
            : this()
        {
            this.name = name;
            this.stack = stk;
        }

        public StackViewItem(string name, ImageStack stk, float[] voxDim)
            : this()
        {
            this.name = name;
            this.stack = stk;
            this.voxDim = voxDim;
        }

        public StackViewItem(string name, ImageStack stk, byte[] mask)
            : this()
        {
            this.name = name;
            this.stack = stk;
            this.mask = mask;
        }

        public StackViewItem(string name, ImageStack stk, Vector2[,] inner, Vector2[,] outer)
            : this()
        {
            this.name = name;
            this.stack = stk;
            this.inner = inner;
            this.outer = outer;
        }

        public string name;
        public ImageStack stack;

        public byte[] mask = null;
        public Vector2[,] inner = null;
        public Vector2[,] outer = null;
        public Point3f[] center;
        public Point3f t0, t1;
        public float[] voxDim;

        public Point3f TransformPoint(Vector2 pt, int slice)
        {
            int idx = Math.Min(slice, center.Length-1);
            Point3f ret = center[idx] + (pt.X - stack.Width / 2) * t0 + (pt.Y - stack.Height / 2) * t1;
            return ret;
        }
    }
}
