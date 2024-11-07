using CorticalExtract.DataStructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorticalExtract.Processing
{
    public class SliceMedianFilter
    {
        public SliceMedianFilter(int radius)
        {
            this.radius = radius;
        }

        int radius;

        void GatherNeighborhood(ImageStack src, int x, int y, int z, float[] buffer)
        {
            int idx = 0;
            for (int i = -radius; i <= radius; i++)
            {
                for (int j = -radius; j <= radius; j++)
                {
                    for (int k = -radius; k <= radius; k++)
                    {
                        buffer[idx] = src[x + i, y + j, k + z];
                        idx++;
                    }
                }
            }
        }

        public ImageStack Process(ImageStack src)
        {
            int w = src.Width;
            int h = src.Height;
            int midSlice = (src.Slices - 1) / 2;

            ImageStack ret = new ImageStack(h, w, 1, new float[3] { 1, 1, 1 });

            Parallel.For(0, h, (j) =>
            //for (int j = 0; j < h; j++)
            {
                int bufferSize = (2 * radius + 1) * (2 * radius + 1) * (2 * radius + 1);
                float[] buffer = new float[bufferSize];
                int medianIdx = (buffer.Length - 1) / 2;

                for (int i = 0; i < w; i++)
                {
                    GatherNeighborhood(src, i, j, midSlice, buffer);
                    Array.Sort(buffer);
                    ret[i, j, 0] = buffer[medianIdx];
                }
            });

            return ret;
        }
    }
}
