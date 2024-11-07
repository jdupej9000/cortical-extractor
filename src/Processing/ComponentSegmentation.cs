using CorticalExtract.DataStructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorticalExtract.Processing
{
    public class ComponentSegmentation
    {
        public ComponentSegmentation(ImageStack stack)
        {
            this.stack = stack;
            height = stack.Height;
            width = stack.Width;

            ActiveSlice = 0;
            InternalThreshold = 400;

            InitMask();
        }

        private struct Pair
        {
            public Pair(int x, int y)
            {
                this.x = x;
                this.y = y;
            }

            public int x, y;
        }

        public delegate bool SegmentRule(ImageStack stk, int x, int y, int z);

        protected ImageStack stack;
        protected byte[] mask;
        protected int width, height;
        protected int activeSlice;
        protected float internalThresh;

        public int ActiveSlice
        {
            get { return activeSlice; }
            set { activeSlice = value; }
        }

        public float InternalThreshold
        {
            get { return internalThresh; }
            set { internalThresh = value; }
        }

        public Point2f GetLargestSegmentCentroid()
        {
            CleanMask();
            FindSegments(SegmentRuleAboveEq);
            return SegmentCentroid(GetLargestIndex(true));
        }


        bool SegmentRuleAboveEq(ImageStack stk, int x, int y, int z)
        {
            return stk[x, y, z] > internalThresh;
        }


        private void InitMask()
        {
            mask = new byte[height * width];
            CleanMask();
        }

        private void CleanMask()
        {
            for (int i = 0; i < height * width; i++)
                mask[i] = 0;
        }

        private byte GetMask(int x, int y)
        {
            return mask[x + y * width];
        }

        private void SetMask(int x, int y, byte v)
        {
            mask[x + y * width] = v;
        }
                
        private int FindSegments(SegmentRule sr)
        {
            byte segmentIdx = 1;

            for (int j = 0; j < height; j++)
            {
                for (int i = 0; i < width; i++)
                {
                    if (GetMask(i, j) == 0 & 
                        sr(stack, i, j, activeSlice))
                    {                        
                        if(FloodFill(i, j, segmentIdx, sr))
                            segmentIdx++;                        
                    }
                }
            }

            return segmentIdx;
        }

        private bool FloodFill(int x, int y, byte id, SegmentRule sr)
        {
            Stack<Pair> lifo = new Stack<Pair>();
            lifo.Push(new Pair(x,y));

            bool goodSegment = false;

            while (lifo.Count > 0)
            {
                Pair hash = lifo.Pop();
                int x1 = hash.x;
                int y1 = hash.y;

                if (x1 < 0 | x1 >= width | y1 < 0 | y1 >= height) continue;
                if (GetMask(x1, y1) != 0) continue;
                if (!sr(stack, x1, y1, activeSlice)) continue;

                goodSegment = true;
                SetMask(x1, y1, id);
                lifo.Push(new Pair(x1 - 1, y1));
                lifo.Push(new Pair(x1 + 1, y1));
                lifo.Push(new Pair(x1, y1 - 1));
                lifo.Push(new Pair(x1, y1 + 1));
                
            }

            return goodSegment;
        }

        private byte GetLargestIndex(bool ignoreZero)
        {
            int[] hist = new int[256];
            
            for (int i = 0; i < 256; i++) hist[i] = 0;

            for (int i = 0; i < height * width; i++)            
                hist[mask[i]]++;

            byte bestIdx = 0;
            byte startIdx = 0;

            if (ignoreZero)
            {
                bestIdx = 1;
                startIdx = 1;
            }

            for (byte i = startIdx; i < 255; i++)            
                if (hist[bestIdx] < hist[i])
                    bestIdx = i;
            
            return bestIdx;
        }

        private Point2f SegmentCentroid(byte idx)
        {
            double x = 0, y = 0;
            int n = 0;

            for (int j = 0; j < height; j++)
            {
                for (int i = 0; i < width; i++)
                {
                    if (GetMask(i,j) == idx)
                    {
                        x += i;
                        y += j;
                        n++;
                    }
                }
            }

            if (n == 0) return new Point2f();

            return new Point2f(
                (float)(x / (double)n),
                (float)(y / (double)n));
        }
    }
}
