using System;
using System.Numerics;

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

        public Vector3[] Process(Vector3[] a)
        {
            int n = a.Length;
            Vector3[] ret = new Vector3[n];

            for (int i = 0; i < n; i++)
            {
                float weightSum = 0;
                float x = 0, y = 0, z = 0;

                for (int j = 0; j < n; j++)
                {
                    float dist = j - i;
                    float weight = MathF.Exp(-dist * dist / beta);
                    x += weight * a[j].X;
                    y += weight * a[j].Y;
                    z += weight * a[j].Z;
                    weightSum += weight;
                }

                ret[i] = new Vector3(x / weightSum, y / weightSum, z / weightSum);
            }

            return ret;
        }
    }
}
