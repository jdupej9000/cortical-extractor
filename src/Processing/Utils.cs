using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace CorticalExtract.Processing
{
    public static class Utils
    {
        public delegate float MatrixQueryDelegate(int i, int j);

        public static int Clamp(int v, int min, int max)
        {
            if (v < min) return min;
            if (v > max) return max;
            return v;
        }

        public static float Blend(float a, float b, float r)
        {
            return (1 - r) * a + r * b;
        }

        public static Vector3[] Seq(Vector3 from, Vector3 to, int k)
        {
            Vector3[] ret = new Vector3[k];

            Vector3 d = (to - from) * (1.0f / (float)(k - 1));

            for (int i = 0; i < k; i++)
                ret[i] = from + (float)i * d;

            return ret;
        }

        public static float[] Seq(float from, float to, int k)
        {
            float[] ret = new float[k];

            float d = (to - from) * (1.0f / (float)(k - 1));

            for (int i = 0; i < k; i++)
                ret[i] = from + (float)i * d;

            return ret;
        }

        public static float ImputeCyclic(float[] x, int idx, int radius, float def = 0, bool addDef = false)
        {
            List<float> nonNa = new List<float>();

            for (int i = -radius; i <= radius; i++)
            {
                int ii = i + radius;
                if (ii < 0) ii += x.Length;
                if (ii >= x.Length) ii -= x.Length;

                if (!float.IsNaN(x[ii])) nonNa.Add(x[ii]);
                else if (addDef) nonNa.Add(def);
            }

            if (nonNa.Count == 0) return def;

            return Mean(nonNa.ToArray());
        }

        public static float Mean(float[] x)
        {
            int n = x.Length;
            float ret = 0.0f;

            for (int i = 0; i < n; i++)
                ret += x[i];

            return ret / (float)n;
        }

        public static void OlsFit(float[] x, float[] y, out float a, out float b)
        {
            int n = x.Length;

            float muX = Mean(x);
            float muY = Mean(y);
            float rxy = 0, sy = 0, sx = 0;

            for (int i = 0; i < n; i++)
            {
                rxy += (x[i] - muX) * (y[i] - muY);
                sx += (x[i] - muX) * (x[i] - muX);
                sy += (y[i] - muY) * (y[i] - muY);
            }

            rxy /= n;
            sx = (float)Math.Sqrt(sx / n);
            sy = (float)Math.Sqrt(sy / n);
            rxy /= sx * sy;

            b = rxy * sy / sx;
            a = muY - b * muX;
        }

        public static float[] ExtractCoordinate(Vector3[] x, int c)
        {
            int n = x.Length;
            float[] ret = new float[n];

            for (int i = 0; i < n; i++)
            {
                if (c == 0) ret[i] = x[i].X;
                else if (c == 1) ret[i] = x[i].Y;
                else if (c == 2) ret[i] = x[i].Z;
                else ret[i] = 0;
            }

            return ret;
        }

        public static float Sqr(float x)
        {
            return x * x;
        }

        public static float PointSegmentDistance(Vector2 a0, Vector2 a1, Vector2 b, out Vector2 bb)
        {
            float l2 = Vector2.DistanceSquared(a0, a1);
            if (l2 <= float.Epsilon)
            {
                bb = a0;
                return Vector2.Distance(a0, b);
            }

            float t = Vector2.Dot(b - a0, a1 - a0) / l2;
            if (t < 0)
            {
                bb = a0;
                return Vector2.Distance(b, a0);
            }
            else if (t > 1)
            {
                bb = a1;
                return Vector2.Distance(b, a1);
            }

            Vector2 proj = a0 + t * (a1 - a0);
            bb = proj;
            return Vector2.Distance(b, proj);
        }

        public static float TriangleArea(Vector3 a, Vector3 b, Vector3 c)
        {
            return 0.5f * Vector3.Cross(b - a, c - a).Length();
        }

        public static Vector3 PointFromPlane(Vector2 pt, Vector3 t0, Vector3 t1)
        {
            return pt.X * t0 + pt.Y * t1;
        }

        public static Vector3 PointAniso(Vector3 pt, float[] voxDim)
        {
            return new Vector3(pt.X * voxDim[0], pt.Y * voxDim[1], pt.Z * voxDim[2]);
        }

        public static float TriangleAreaAniso(Vector2 a, Vector2 b, Vector2 c, Vector3 t0, Vector3 t1, float[] voxDim)
        {
            return TriangleArea(PointAniso(PointFromPlane(a, t0, t1), voxDim),
                PointAniso(PointFromPlane(b, t0, t1), voxDim),
                PointAniso(PointFromPlane(c, t0, t1), voxDim));
        }

        public static string MatrixToString(int rows, int cols, MatrixQueryDelegate m)
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    if (j < cols - 1) sb.AppendFormat("{0},", m(i, j));
                    else sb.AppendFormat("{0}\n", m(i, j));
                }
            }

            return sb.ToString();
        }


        public static float FastMedian27(float[] a)
        {
            float[] left = new float[14], right = new float[14];
            float median;
            int nLeft, nRight;
            int pa = 0, pp;

            // pick first value as median candidate
            pp = pa;
            median = a[pp++];
            nLeft = nRight = 1;

            for (; ; )
            {
                // get next value
                float val = a[pp++];

                // if value is smaller than median, append to left heap
                if (val < median)
                {
                    // move biggest value to the heap top
                    int child = nLeft++, parent = (child - 1) / 2;
                    while (parent != 0 && val > left[parent])
                    {
                        left[child] = left[parent];
                        child = parent;
                        parent = (parent - 1) / 2;
                    }
                    left[child] = val;

                    // if left heap is full
                    if (nLeft == 14)
                    {
                        // for each remaining value
                        for (int nVal = 27 - (pp - pa); nVal > 0; --nVal)
                        {
                            // get next value
                            val = a[pp++];

                            // if value is to be inserted in the left heap
                            if (val < median)
                            {
                                child = left[2] > left[1] ? 2 : 1;
                                if (val >= left[child])
                                    median = val;
                                else
                                {
                                    median = left[child];
                                    parent = child;
                                    child = parent * 2 + 1;
                                    while (child < 14)
                                    {
                                        if (child < 13 && left[child + 1] > left[child])
                                            ++child;
                                        if (val >= left[child])
                                            break;
                                        left[parent] = left[child];
                                        parent = child;
                                        child = parent * 2 + 1;
                                    }
                                    left[parent] = val;
                                }
                            }
                        }
                        return median;
                    }
                }

                // else append to right heap
                else
                {
                    // move smallest value to the heap top
                    int child = nRight++, parent = (child - 1) / 2;
                    while (parent != 0 && val < right[parent])
                    {
                        right[child] = right[parent];
                        child = parent;
                        parent = (parent - 1) / 2;
                    }
                    right[child] = val;

                    // if right heap is full
                    if (nRight == 14)
                    {
                        // for each remaining value
                        for (int nVal = 27 - (pp - pa); nVal > 0; --nVal)
                        {
                            // get next value
                            val = a[pp++];

                            // if value is to be inserted in the right heap
                            if (val > median)
                            {
                                child = right[2] < right[1] ? 2 : 1;
                                if (val <= right[child])
                                    median = val;
                                else
                                {
                                    median = right[child];
                                    parent = child;
                                    child = parent * 2 + 1;
                                    while (child < 14)
                                    {
                                        if (child < 13 && right[child + 1] < right[child])
                                            ++child;
                                        if (val <= right[child])
                                            break;
                                        right[parent] = right[child];
                                        parent = child;
                                        child = parent * 2 + 1;
                                    }
                                    right[parent] = val;
                                }
                            }
                        }
                        return median;
                    }
                }
            }
        }

    }
}
