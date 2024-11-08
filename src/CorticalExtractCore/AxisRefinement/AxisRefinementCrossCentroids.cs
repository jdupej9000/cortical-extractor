using CorticalExtract.DataStructures;
using CorticalExtract.Processing;
using System;
using System.Numerics;

namespace CorticalExtract.AxisRefinement
{
    class AxisRefinementCrossCentroids : IAxisRefinement
    {
        public AxisRefinementCrossCentroids()
            : this(new PassthroughPathSmoothing())
        {
        }

        public AxisRefinementCrossCentroids(IPathSmooting smoothing, bool naive = false)
        {
            NaiveCentering = naive;
            Tolerance = 0.1f;
            MaxIter = 10;
            this.smoothing = smoothing;
        }

        protected IPathSmooting smoothing;

        public bool NaiveCentering
        {
            get;
            set;
        }

        public float Tolerance
        {
            get;
            set;
        }

        public int MaxIter
        {
            get;
            set;
        }

        public Vector2 FindSliceCentroidNaiveInternal(ImageStack stk, int slice, Vector2 center, int maxRadius, float thresh = 500)
        {
            int radius = Math.Min(maxRadius, Math.Min(stk.Width / 2 - 1, stk.Height / 2 - 1));
            int numBone = 0;
            int accumX = 0, accumY = 0;

            for (int j = -radius; j < radius + 1; j++)
            {
                for (int i = -radius; i < radius + 1; i++)
                {
                    if (stk.SampleSlice(center.X + i, center.Y + j, slice) > thresh)
                    {
                        accumX += i;
                        accumY += j;
                        numBone++;
                    }
                }
            }

            if (numBone > 0)
                return new Vector2((float)accumX / (float)numBone, (float)accumY / (float)numBone);
            else
                return new Vector2(0, 0);
        }

        public Vector2 FindSliceCentroidNaive(ImageStack stk, int slice, int maxRadius, float thresh = 500)
        {
            return FindSliceCentroidNaiveInternal(stk, slice, new Vector2(stk.Width / 2, stk.Height / 2), maxRadius, thresh);
        }

        public Vector2 FindSliceCentroidIterative(ImageStack stk, int slice, int maxRadius, float thresh = 500, float tol = 0.1f, int maxIt = 10)
        {
            Vector2 center = new Vector2(stk.Width / 2, stk.Height / 2);
            Vector2 x0 = center;
            int it = 0;

            while (it < maxIt)
            {
                Vector2 dx = FindSliceCentroidNaiveInternal(stk, slice, x0, maxRadius, thresh);
                x0 += dx;

                float err = dx.Length();

                if (err < tol) break;
                it++;
            }

            return x0 - center;
        }

        public Vector2 FindSliceCentroidConnected(ImageStack stk, int slice, float thresh = 500)
        {
            ComponentSegmentation seg = new ComponentSegmentation(stk);
            seg.ActiveSlice = slice;
            seg.InternalThreshold = thresh;

            Vector2 segmentCenter = seg.GetLargestSegmentCentroid();
            Vector2 center = new Vector2(stk.Width / 2, stk.Height / 2);

            return segmentCenter - center;
        }

        public Vector3[] Process(ImageStack stk, Vector3 normal, Vector3 binormal, Vector3[] origins)
        {
            int n = origins.Length;
            Vector3[] ret = new Vector3[n];

            for (int i = 0; i < n; i++)
            {
                Vector2 sliceCenter = NaiveCentering ?
                    FindSliceCentroidNaive(stk, i, 40) :
                    //FindSliceCentroidIterative(stk, i, 22, 400, Tolerance, MaxIter);
                    FindSliceCentroidConnected(stk, i, 400);
                Vector3 refined = origins[i] + sliceCenter.X * normal + sliceCenter.Y * binormal;
                ret[i] = refined;
            }

            return smoothing.Process(ret);
        }
    }
}
