﻿using CorticalExtract.DataStructures;
using CorticalExtract.Processing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorticalExtract.AxisRefinement
{
    public class LinearPathSmoothing : IPathSmooting
    {
        public LinearPathSmoothing()
        {
        }

        public static float[] LineParams(Point3f[] y, float[] x)
        {
            float ax, bx, ay, by, az, bz;
            int n = y.Length;

            Utils.OlsFit(x, Utils.ExtractCoordinate(y, 0), out ax, out bx);
            Utils.OlsFit(x, Utils.ExtractCoordinate(y, 1), out ay, out by);
            Utils.OlsFit(x, Utils.ExtractCoordinate(y, 2), out az, out bz);

            return new float[6] { ax, bx, ay, by, az, bz };
        }

        public Point3f[] Process(Point3f[] y)
        {
            int n = y.Length;
            float[] x = Utils.Seq(0, n-1, n);
            float[] p = LineParams(y, x);

            Point3f[] ret = new Point3f[n];
            for (int i = 0; i < n; i++)
            {
                ret[i] = new Point3f(p[0] + x[i] * p[1], p[2] + x[i] * p[3], p[4] + x[i] * p[5]);
            }

            return ret;
        }
    }
}