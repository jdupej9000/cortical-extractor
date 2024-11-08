using CorticalExtract.DataStructures;
using System.Collections.Generic;
using System.Numerics;

namespace CorticalExtract.Processing
{
    public struct RayMarchResult
    {
        public RayMarchResult(float r)
        {
            this.r = r;
            thresh = 0;
            valid = true;
        }

        public RayMarchResult(float r, float thresh)
        {
            this.r = r;
            this.thresh = thresh;
            valid = true;
        }

        public bool valid;
        public float r;
        public float thresh;
    }


    public class BoundarySegmentation
    {
        public BoundarySegmentation(ImageStack stk, int slice, float maxRadius)
        {
            stack = stk;
            this.slice = slice;
            this.maxRadius = maxRadius;

            halfMaxRadius = 4;
            minimumRun = 0.5f;
        }

        ImageStack stack;
        int slice;
        float maxRadius;
        float minimumRun;
        int halfMaxRadius;

        const float DELTA = 0.25f;
        public delegate float RayCastDelegate(Vector2 o, Vector2 d, float t0, float thr);

        public int ActiveSlice
        {
            get { return slice; }
            set { slice = value; }
        }

        public float MaxRadius
        {
            get { return maxRadius; }
            set { maxRadius = value; }
        }

        public int HmhRadius
        {
            get { return halfMaxRadius; }
            set { halfMaxRadius = value; }
        }

        public bool VerifyRise(Vector2 origin, Vector2 direction, float t0, float thresh, float minRun = 3.0f)
        {
            float t = 0;

            while (t < minRun)
            {
                float f = stack.SampleSlice(origin.X + (t0 + t) * direction.X,
                    origin.Y + (t0 + t) * direction.Y, slice);

                if (f < thresh) return false;

                t += DELTA;
            }

            return true;
        }

        public float RayCastRise(Vector2 origin, Vector2 direction, float t0, float thresh)
        {
            float f0 = stack.SampleSlice(origin.X + t0 * direction.X, origin.Y + t0 * direction.Y, slice);
            float t = t0 + DELTA;

            while (t < maxRadius)
            {
                float f1 = stack.SampleSlice(origin.X + t * direction.X, origin.Y + t * direction.Y, slice);

                if (f0 < thresh && f1 >= thresh)
                {
                    if (VerifyRise(origin, direction, t, thresh, minimumRun))
                    {
                        float r = (thresh - f0) / (f1 - f0) * DELTA;
                        return t - DELTA + r;
                    }
                }

                f0 = f1;
                t += DELTA;
            }

            return MaxRadius;
        }

        public float RayCastFall(Vector2 origin, Vector2 direction, float t0, float thresh)
        {
            return RayCastFall(origin, direction, t0, thresh, thresh);
        }

        public float RayCastFall(Vector2 origin, Vector2 direction, float t0, float thresh1, float thresh2)
        {
            float f0 = stack.SampleSlice(origin.X + t0 * direction.X, origin.Y + t0 * direction.Y, slice);
            float t = t0 + DELTA;

            while (t < maxRadius)
            {
                float f1 = stack.SampleSlice(origin.X + t * direction.X, origin.Y + t * direction.Y, slice);

                if (f0 >= thresh1 & f1 < thresh2)
                {
                    bool valid = true;
                    for (int q = 0; q < 8; q++)
                    {
                        float tt = DELTA * (float)(q + 1) + t;
                        float f2 = stack.SampleSlice(origin.X + tt * direction.X, origin.Y + tt * direction.Y, slice);
                        if (f2 > thresh2) valid = false;
                    }

                    if (valid)
                    {
                        float r = (thresh1 - f1) / (f0 - f1) * DELTA;
                        return t - r;
                    }
                }

                f0 = f1;
                t += DELTA;
            }

            return MaxRadius;
        }

        public float RayCastFallReverse(Vector2 origin, Vector2 direction, float t0, float thresh)
        {
            float f0 = stack.SampleSlice(origin.X + t0 * direction.X, origin.Y + t0 * direction.Y, slice);
            float t = t0 - DELTA;

            //if (f0 < thresh) return t0;

            while (t > 0)
            {
                float f1 = stack.SampleSlice(origin.X + t * direction.X, origin.Y + t * direction.Y, slice);

                if (f0 >= thresh & f1 < thresh)
                {
                    bool valid = true;
                    /*for (int q = 0; q < 8; q++)
                    {
                        float tt = DELTA * (float)(-q - 1) + t;
                        float f2 = stack.SampleSlice(origin.X + tt * direction.X, origin.Y + tt * direction.Y, slice);
                        if (f2 > thresh) valid = false;
                    }*/

                    if (valid)
                    {
                        float r = (thresh - f1) / (f0 - f1) * DELTA;
                        return t - r;
                    }
                }

                f0 = f1;
                t -= DELTA;
            }

            return 0;
        }

        public RayMarchResult HalfMaxHeight(Vector2 origin, Vector2 direction, float t0, float thr0, RayCastDelegate rc, bool outside = true)
        {
            return HalfMaxHeight(origin, direction, t0, thr0, rc, outside, halfMaxRadius, halfMaxRadius);
        }

        public RayMarchResult HalfMaxHeight(Vector2 origin, Vector2 direction, float t0, float thr0, RayCastDelegate rc, bool outside, int hmhRadiusIn, int hmhRadiusOut)
        {
            float r0 = rc(origin, direction, t0, thr0);

            if (hmhRadiusIn == 0 && hmhRadiusOut == 0) return new RayMarchResult(r0, thr0);

            float min = thr0, max = thr0;

            for (int i = -hmhRadiusIn; i <= hmhRadiusOut; i++)
            {
                float thmh = r0 + i;
                float v = stack.SampleSlice(origin.X + thmh * direction.X, origin.Y + thmh * direction.Y, slice);
                if (v < min) min = v;
                if (v > max) max = v;
            }

            float halfMax = 0.5f * (min + max);

            RayMarchResult ret = new RayMarchResult(
                rc(origin, direction, t0, halfMax),
                halfMax);

            //return rc(origin, direction, outside? r0 - halfMaxRadius : r0 + halfMaxRadius, halfMax);
            return ret;
        }

        public float AutoThreshold(Vector2 origin, Vector2 direction, float maxRadius)
        {
            float t = 0;
            List<float> path = new List<float>();

            float q = 0.85f;

            while (t < maxRadius)
            {
                float f = stack.SampleSlice(origin.X + t * direction.X, origin.Y + t * direction.Y, slice);
                path.Add(f);
                t += DELTA;
            }

            path.Sort();

            int idx = (int)(((float)path.Count) * q);

            float thr = path[idx];

            return thr;
        }
    }
}
