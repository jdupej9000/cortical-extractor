using CorticalExtract.Processing;
using System;
using System.IO;
using System.Numerics;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CorticalExtract.DataStructures
{
    public class ImageStack
    {
        public enum VoxelFormat
        {
            UShortLE,
            ShortLE,
            ShortBE,
            FloatLE,
            FloatBE
        }

        public static ImageStack FromFile(string path, int width, int height, int slices, VoxelFormat fmt)
        {
            return FromFile(path, width, height, slices, fmt, new float[3] { 1, 1, 1 });
        }

        public static ImageStack FromFile(string path, int width, int height, int slices, VoxelFormat fmt, float[] voxDim, int[] sliceSelect=null)
        {   
            int[] sel = null;

            if (sliceSelect == null)
            {
                sel = new int[slices];
                for (int i = 0; i < slices; i++) sel[i] = i;
            }
            else
            {
                sel = sliceSelect;
            }
            
            ImageStack ret = new ImageStack(height, width, sel.Length, voxDim);

            try
            {
                int itemSize = 2;
                if (fmt == VoxelFormat.FloatBE | fmt == VoxelFormat.FloatLE) itemSize = 4;
                int buffSize = width * height * itemSize;

                BinaryReader sr = new BinaryReader(new FileStream(path, FileMode.Open, FileAccess.Read));
                byte[] buffer = new byte[buffSize];

                int curSliceIdx = 0;
                for (int i = 0; i < slices; i++)
                {
                    sr.Read(buffer, 0, buffSize);

                    if (i == sel[curSliceIdx])
                    {
                        ret.SetSliceFromBinary(curSliceIdx, buffer, fmt);
                        curSliceIdx++;
                        if (curSliceIdx >= sel.Length) break;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            return ret;
        }



        public ImageStack(int height, int width, int slices, float[] voxDim)
        {
            this.height = height;
            this.width = width;
            this.slices = slices;
            this.voxDim = voxDim;

            data = new float[slices][];
            for (int i = 0; i < slices; i++)
                data[i] = new float[width * height];
        }

        public ImageStack(float[,] arr)
        {
            this.height = arr.GetLength(0);
            this.width = arr.GetLength(1);
            this.slices = 1;

            data = new float[1][];
            data[0] = new float[width * height];

            for (int i = 0; i < height; i++)
                for (int j = 0; j < width; j++)
                    this[i, j, 0] = arr[i, j];
        }

        public delegate float ApplyFunction(ImageStack stk, int x, int y, int z);

        int height, width, slices;
        float[] voxDim;
        float[][] data;

        public int Slices
        {
            get { return slices; }
        }

        public int Height
        {
            get { return height; }
        }

        public int Width
        {
            get { return width; }
        }

        int Idx(int x, int y)
        {
            return Utils.Clamp(x, 0, width - 1) +
                width * Utils.Clamp(y, 0, height - 1);
        }

        public float this[int x, int y, int z]
        {
            get { return data[Utils.Clamp(z, 0, slices-1)][Idx(x, y)]; }
            set { data[Utils.Clamp(z, 0, slices-1)][Idx(x, y)] = value; }
        }

        public float Sample(Vector3 p)
        {
            return Sample(p.X, p.Y, p.Z);
        }

        public float Sample(float x, float y, float z)
        {
            float rx = x / voxDim[0];
            float ry = y / voxDim[1];
            float rz = z / voxDim[2];

            int ix = (int)Math.Floor(rx);
            int iy = (int)Math.Floor(ry);
            int iz = (int)Math.Floor(rz);

            rx -= ix;
            ry -= iy;
            rz -= iz;

            float a00 = Utils.Blend(this[ix, iy, iz], this[ix + 1, iy, iz], rx);
            float a01 = Utils.Blend(this[ix, iy + 1, iz], this[ix + 1, iy + 1, iz], rx);
            float a10 = Utils.Blend(this[ix, iy, iz + 1], this[ix + 1, iy, iz + 1], rx);
            float a11 = Utils.Blend(this[ix, iy + 1, iz + 1], this[ix + 1, iy + 1, iz + 1], rx);

            float a0 = Utils.Blend(a00, a01, ry);
            float a1 = Utils.Blend(a10, a11, ry);

            float a = Utils.Blend(a0, a1, rz);

            return a;
        }

        public float SampleSlice(float x, float y, int z)
        {
            float rx = x / voxDim[0];
            float ry = y / voxDim[1];            

            int ix = (int)Math.Floor(rx);
            int iy = (int)Math.Floor(ry);            

            rx -= ix;
            ry -= iy;

            float a0 = Utils.Blend(this[ix, iy, z], this[ix + 1, iy, z], rx);
            float a1 = Utils.Blend(this[ix, iy + 1, z], this[ix + 1, iy + 1, z], rx);

            float a = Utils.Blend(a0, a1, ry);
            
            return a;
        }

        public ImageStack Apply(ApplyFunction fun)
        {
            ImageStack ret = new ImageStack(height, width, slices, voxDim);

            Parallel.For(0, slices, (k) =>
                {
                    for (int j = 0; j < height; j++)
                    {
                        for (int i = 0; i < width; i++)
                        {
                            ret[i, j, k] = fun(this, i, j, k);
                        }
                    }
                });

            return ret;
        }

        public void OffsetAll(float delta)
        {
            int n = height * width;
            for (int j = 0; j < slices; j++)
                for (int i = 0; i < n; i++)
                    data[j][i] += delta;
        }


        void SetSliceFromBinary(int slice, byte[] data, VoxelFormat fmt)
        {
            int currentIdx = 0;
            int dataIdx = 0;

            for (int j = 0; j < height; j++)
            {
                for (int i = 0; i < width; i++)
                {
                    float val = 0f;

                    switch (fmt)
                    {
                        case VoxelFormat.FloatLE:
                            val = BitConverter.ToSingle(data, dataIdx);
                            dataIdx += 4;
                            break;

                        case VoxelFormat.FloatBE:
                            val = BitConverter.ToSingle(new byte[4] { data[dataIdx + 3], data[dataIdx + 2], data[dataIdx + 1], data[dataIdx] }, 0);
                            dataIdx += 4;
                            break;

                        case VoxelFormat.UShortLE:
                            val = BitConverter.ToUInt16(data, dataIdx);
                            dataIdx += 2;
                            break;

                        case VoxelFormat.ShortLE:
                            val = BitConverter.ToInt16(data, dataIdx);
                            dataIdx += 2;
                            break;

                        case VoxelFormat.ShortBE:
                            val = BitConverter.ToInt16(new byte[2] { data[dataIdx + 1], data[dataIdx] }, 0);
                            dataIdx += 2;
                            break;
                    }

                    this.data[slice][currentIdx] = val;
                    currentIdx++;
                }
            }


        }
    }
}
