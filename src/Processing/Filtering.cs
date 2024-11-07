using CorticalExtract.DataStructures;
using System;

namespace CorticalExtract.Processing
{
    public static class Filtering
    {
        public static ImageStack MedianFilter(ImageStack stk, int radius)
        {
            ImageStack ret = stk.Apply((s, i, j, k) =>
                {
                    int m = 2 * radius + 1;
                    m = m * m * m;
                    float[] arr = new float[m];
                    m = 0;

                    for (int x = -radius; x <= radius; x++)
                        for (int y = -radius; y <= radius; y++)
                            for (int z = -radius; z <= radius; z++)
                                arr[m++] = s[i + x, j + y, k + z];

                    float filt = 0;

                    if (radius == 1)
                    {
                        filt = Utils.FastMedian27(arr);
                    }
                    else
                    {
                        Array.Sort(arr);
                        int m2 = arr.Length / 2;
                        filt = arr[m2];
                    }

                    return filt;
                });

            return ret;
        }
    }
}
