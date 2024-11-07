using CorticalExtract.DataStructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorticalExtract.AxisRefinement
{
    public class GaussianPathSmoothing : IPathSmooting
    {
        public GaussianPathSmoothing()
            : this(0.1f)
        {
        }

        public GaussianPathSmoothing(float beta)
        {
            this.beta = beta;
        }

        float beta;

        public Point3f[] Process(Point3f[] a)
        {
            int n = a.Length;
            Point3f[] ret = new Point3f[n];

            for (int i = 0; i < n; i++)
            {
                float weightSum = 0;
                float x = 0, y = 0, z = 0;

                for (int j = 0; j < n; j++)
                {
                    float dist = j - i;
                    float weight = (float)Math.Exp(-dist * dist / beta);
                    x += weight * a[j].X;
                    y += weight * a[j].Y;
                    z += weight * a[j].Z;
                    weightSum += weight;
                }

                ret[i] = new Point3f(x / weightSum, y / weightSum, z / weightSum);
            }

            return ret;
        }
    }
}
