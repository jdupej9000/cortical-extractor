using System;
using System.Numerics;

namespace CorticalExtract.DataStructures
{
    public class StackViewItem
    {
        private StackViewItem()
        {
            center = new Vector3[] { new Vector3(0, 0, 0) };
            t0 = new Vector3(1, 0, 0);
            t1 = new Vector3(0, 1, 0);
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
        public Vector3[] center;
        public Vector3 t0, t1;
        public float[] voxDim;

        public Vector3 TransformPoint(Vector2 pt, int slice)
        {
            int idx = Math.Min(slice, center.Length - 1);
            Vector3 ret = center[idx] + (pt.X - stack.Width / 2) * t0 + (pt.Y - stack.Height / 2) * t1;
            return ret;
        }
    }
}
