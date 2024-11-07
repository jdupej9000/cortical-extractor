using CorticalExtract.DataStructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorticalExtract.Processing
{
    public class Reslicer
    {
        public Reslicer(ImageStack stk)
        {
            stack = stk;
        }

        protected ImageStack stack;

        public ImageStack Reslice(Point3f[] center, Point3f normal, Point3f binormal, int radius)
        {
            int n = center.Length;

            ImageStack ret = new ImageStack(2 * radius + 1, 2 * radius + 1, n, 
                new float[3] { 1, 1, 1 });

            Parallel.For(0, n, (i) =>
            {
                for (int y = 0; y < 2 * radius + 1; y++)
                {
                    for (int x = 0; x < 2 * radius + 1; x++)
                    {
                        Point3f xij = center[i] + (float)(x - radius) * normal + (float)(y - radius) * binormal;
                        ret[x, y, i] = stack.Sample(xij);
                    }
                }
            });
            
            return ret;
        }
    }
}
