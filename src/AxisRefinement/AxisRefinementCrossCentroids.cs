using CorticalExtract.DataStructures;
using CorticalExtract.Processing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorticalExtract.AxisRefinement
{
    class AxisRefinementCrossCentroids : IAxisRefinement
    {
        public AxisRefinementCrossCentroids()
            : this(new PassthroughPathSmoothing())
        {            
        }

        public AxisRefinementCrossCentroids(IPathSmooting smoothing, bool naive=false)
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

        public Point2f FindSliceCentroidNaiveInternal(ImageStack stk, int slice, Point2f center, int maxRadius, float thresh = 500)
        {
            int radius = Math.Min(maxRadius, Math.Min(stk.Width / 2 - 1, stk.Height / 2 - 1));
            int numBone = 0;
            int accumX = 0, accumY = 0;

            for (int j = -radius; j < radius + 1; j++)
            {
                for (int i = -radius; i < radius + 1; i++)
                {                    
                    if(stk.SampleSlice(center.X + i, center.Y + j, slice) > thresh)
                    {
                        accumX += i;
                        accumY += j;
                        numBone++;
                    }
                }
            }

            if (numBone > 0)
                return new Point2f((float)accumX / (float)numBone, (float)accumY / (float)numBone);
            else
                return new Point2f(0, 0);
        }

        public Point2f FindSliceCentroidNaive(ImageStack stk, int slice, int maxRadius, float thresh=500)
        {
            return FindSliceCentroidNaiveInternal(stk, slice, new Point2f(stk.Width/2, stk.Height/2), maxRadius, thresh);
        }

        public Point2f FindSliceCentroidIterative(ImageStack stk, int slice, int maxRadius, float thresh = 500, float tol=0.1f, int maxIt = 10)
        {
            Point2f center = new Point2f(stk.Width / 2, stk.Height / 2);
            Point2f x0 = new Point2f(center);
            int it = 0;
            
            while (it < maxIt)
            {
                Point2f dx = FindSliceCentroidNaiveInternal(stk, slice, x0, maxRadius, thresh);
                x0 += dx;

                float err = dx.Length;

                if (err < tol) break;
                it++;
            }

            return x0 - center;
        }

        public Point2f FindSliceCentroidConnected(ImageStack stk, int slice, float thresh = 500)
        {
            ComponentSegmentation seg = new ComponentSegmentation(stk);
            seg.ActiveSlice = slice;
            seg.InternalThreshold = thresh;

            Point2f segmentCenter = seg.GetLargestSegmentCentroid();
            Point2f center = new Point2f(stk.Width / 2, stk.Height / 2);

            return segmentCenter - center;
        }

        public Point3f[] Process(ImageStack stk, Point3f normal, Point3f binormal, Point3f[] origins)
        {
            int n = origins.Length;
            Point3f[] ret = new Point3f[n];

            for(int i = 0; i < n; i++)
            {
                Point2f sliceCenter = NaiveCentering ?
                    FindSliceCentroidNaive(stk, i, 40) :
                    //FindSliceCentroidIterative(stk, i, 22, 400, Tolerance, MaxIter);
                    FindSliceCentroidConnected(stk, i, 400);
                Point3f refined = origins[i] + sliceCenter.X * normal + sliceCenter.Y * binormal;
                ret[i] = refined;
            }

            return smoothing.Process(ret);            
        }
    }
}
