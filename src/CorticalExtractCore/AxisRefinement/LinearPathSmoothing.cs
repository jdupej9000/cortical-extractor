using CorticalExtract.Processing;
using System.Numerics;

namespace CorticalExtract.AxisRefinement
{
    public class LinearPathSmoothing : IPathSmooting
    {
        public LinearPathSmoothing()
        {
        }

        public static float[] LineParams(Vector3[] y, float[] x)
        {
            Utils.OlsFit(x, Utils.ExtractCoordinate(y, 0), out float ax, out float bx);
            Utils.OlsFit(x, Utils.ExtractCoordinate(y, 1), out float ay, out float by);
            Utils.OlsFit(x, Utils.ExtractCoordinate(y, 2), out float az, out float bz);

            return new float[6] { ax, bx, ay, by, az, bz };
        }

        public Vector3[] Process(Vector3[] y)
        {
            int n = y.Length;
            float[] x = Utils.Seq(0, n - 1, n);
            float[] p = LineParams(y, x);

            Vector3[] ret = new Vector3[n];
            for (int i = 0; i < n; i++)
            {
                ret[i] = new Vector3(p[0] + x[i] * p[1], p[2] + x[i] * p[3], p[4] + x[i] * p[5]);
            }

            return ret;
        }
    }
}
