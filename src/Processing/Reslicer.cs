using CorticalExtract.DataStructures;
using System.Numerics;
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

        public ImageStack Reslice(Vector3[] center, Vector3 normal, Vector3 binormal, int radius)
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
                        Vector3 xij = center[i] + (float)(x - radius) * normal + (float)(y - radius) * binormal;
                        ret[x, y, i] = stack.Sample(xij);
                    }
                }
            });

            return ret;
        }
    }
}
