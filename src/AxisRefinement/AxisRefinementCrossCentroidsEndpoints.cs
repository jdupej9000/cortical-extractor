﻿using CorticalExtract.DataStructures;
using CorticalExtract.Processing;
using System.Numerics;

namespace CorticalExtract.AxisRefinement
{
    public class AxisRefinementEndpoints
    {
        public AxisRefinementEndpoints(ImageStack stk, float alpha, float beta)
        {
            this.alpha = alpha;
            this.beta = beta;
            this.mainStack = stk;
        }

        float alpha, beta;
        ImageStack mainStack;

        public Vector3[] Process(ImageStack stk, Vector3 normal, Vector3 binormal, Vector3[] origins)
        {
            AxisRefinementCrossCentroids arcc = new AxisRefinementCrossCentroids();

            int n = origins.Length;

            // find per-slice centroids
            Vector3[] centroids = new Vector3[n];
            for (int i = 0; i < n; i++)
            {
                Vector2 sliceCenter = arcc.FindSliceCentroidIterative(stk, i, 30, 500);
                Vector3 refined = origins[i] + sliceCenter.X * normal + sliceCenter.Y * binormal;
                centroids[i] = refined;
            }

            // fit line to centroids
            float[] x = Utils.Seq(alpha, beta, n);
            float[] lineArgs = LinearPathSmoothing.LineParams(centroids, x);
            Vector3 p0 = new Vector3(lineArgs[0], lineArgs[2], lineArgs[4]);
            Vector3 p1 = new Vector3(lineArgs[0] + lineArgs[1], lineArgs[2] + lineArgs[3], lineArgs[4] + lineArgs[5]);

            // adjust centroids to surface with HMH
            float t0 = SearchFallHmh(p0, p1, 0.05f, -0.001f, -0.1f);
            float t1 = SearchFallHmh(p0, p1, 0.95f, 0.001f, 1.1f);
            Vector3 pp0 = p0 + t0 * (p1 - p0);
            Vector3 pp1 = p0 + t1 * (p1 - p0);

            return new Vector3[2] { pp0, pp1 };
        }

        protected float SearchFallHmh(Vector3 p0, Vector3 p1, float t0, float dt, float tlimit, float thresh = 300, int hmhRadius = 4)
        {
            float r0 = SearchFall(p0, p1, t0, dt, tlimit, thresh);

            float min = thresh, max = thresh;
            for (int i = -hmhRadius; i <= hmhRadius; i++)
            {
                float v = mainStack.Sample(p0 + ((float)i * dt + r0) * (p1 - p0));
                if (v > max) max = v;
                if (v < min) min = v;
            }

            float halfMax = 0.5f * (max + min);

            return SearchFall(p0, p1, r0 - (float)hmhRadius * dt, dt, tlimit, halfMax);
        }

        protected float SearchFall(Vector3 p0, Vector3 p1, float t0, float dt, float tlimit, float thresh = 250)
        {
            float f0 = mainStack.Sample(p0 + t0 * (p1 - p0));
            float t = t0 + dt;
            bool forward = dt > 0;

            while (true)
            {
                if (forward & (tlimit < t)) break;
                if (!forward & (tlimit > t)) break;

                float f1 = mainStack.Sample(p0 + t * (p1 - p0));

                if (f0 >= thresh & f1 < thresh)
                {
                    float r = (thresh - f1) / (f0 - f1) * dt;
                    return t + r;
                }

                f0 = f1;
                t += dt;
            }

            return tlimit;
        }
    }
}
