using CorticalExtract.DataStructures;
using System.Collections.Generic;
using System.Linq;

namespace CorticalExtract.Processing
{
    public class ThresholdClassification
    {
        public struct ThresholdRange
        {
            public ThresholdRange(float t0, float t1)
            {
                min = t0;
                max = t1;
            }

            float min, max;

            public bool IsWithin(float t)
            {
                return (min <= t) & (max > t);
            }
        }



        public ThresholdClassification()
        {
        }

        Dictionary<byte, ThresholdRange> ranges = new Dictionary<byte, ThresholdRange>();
        long[] accum = new long[256];

        public void AddRange(byte code, ThresholdRange range)
        {
            ranges.Add(code, range);
        }

        public void CleanRanges()
        {
            ranges.Clear();
        }

        public byte[] GetMaskSlice(ImageStack stk, int slice)
        {
            byte[] codes = ranges.Keys.ToArray();
            ThresholdRange[] thr = ranges.Values.ToArray();

            int numThr = codes.Length;
            int w = stk.Width;
            int h = stk.Height;
            byte[] ret = new byte[w * h];

            int idx = 0;

            for (int j = 0; j < h; j++)
            {
                for (int i = 0; i < w; i++)
                {
                    ret[idx] = 0;

                    for (byte k = 0; k < numThr; k++)
                    {
                        float v = stk[i, j, slice];
                        if (thr[k].IsWithin(v))
                        {
                            ret[idx] = codes[k];
                            break;
                        }
                    }
                    idx++;
                }
            }

            return ret;
        }

        public void CleanAccum()
        {
            for (int i = 0; i < 256; i++)
                accum[i] = 0;
        }

        public void AccumulateMask(byte[] mask)
        {
            int n = mask.Length;

            for (int i = 0; i < n; i++)
                accum[mask[i]]++;
        }

        public long GetAccumCount(byte idx)
        {
            return accum[idx];
        }
    }
}
