using CorticalExtract.DataStructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace CorticalExtract.Processing
{
    public class ExtractorSetup
    {
        public ExtractorSetup(string configLine)
        {
            // source.raw, width, height, slices, format, voxDimX, voxDimY, voxDimZ, offset, refine, refine.par, lm0x, lm0y, lm0z, lm1x, lm1y, lm1z, lmdx, lmdy, lmdz, alpha, beta, k, dest.profile, dest.axis
            string[] parts = configLine.Split(',');

            pathRaw = parts[0];
            ParseInt(parts[1], 512, out width);
            ParseInt(parts[2], 512, out height);
            ParseInt(parts[3], 1, out slices);
            format = ParseFormat(parts[4], ImageStack.VoxelFormat.ShortLE);
            voxDim = new float[3] { 1, 1, 1 };
            ParseFloat(parts[5], voxDim[0], out voxDim[0]);
            ParseFloat(parts[6], voxDim[1], out voxDim[1]);
            ParseFloat(parts[7], voxDim[2], out voxDim[2]);
            ParseFloat(parts[8], 0, out offset);
            ParseVector3(parts[9], parts[10], parts[11], Vector3.Zero, out lm0);
            ParseVector3(parts[12], parts[13], parts[14], Vector3.Zero, out lm1);
            ParseVector3(parts[15], parts[16], parts[17], Vector3.Zero, out lmd);
            pathDestProfile = parts[18];
            pathDestAxis = parts[19];
            pathDestSegments = parts[20];
        }

        public string pathRaw;
        public int width, height, slices;
        public ImageStack.VoxelFormat format;
        public float[] voxDim;
        public float offset;
        public string refinement;
        public float refineParam;
        public Vector3 lm0, lm1, lmd;
        public string pathDestProfile, pathDestAxis, pathDestSegments;
        public float alpha, beta;
        public int outSlices;
        public bool useEndpointRefinement;

        void ParseInt(string text, int def, out int dest)
        {
            if (!int.TryParse(text, out dest))
                dest = def;
        }

        void ParseFloat(string text, float def, out float dest)
        {
            if (!float.TryParse(text, out dest))
                dest = def;
        }

        void ParseBool(string text, bool def, out bool dest)
        {
            if (!bool.TryParse(text, out dest))
                dest = def;
        }

        ImageStack.VoxelFormat ParseFormat(string text, ImageStack.VoxelFormat def)
        {
            switch (text)
            {
                case "u16": return ImageStack.VoxelFormat.UShortLE;
                case "i16": return ImageStack.VoxelFormat.ShortLE;
                case "i16b": return ImageStack.VoxelFormat.ShortBE;
                case "f32": return ImageStack.VoxelFormat.FloatLE;
                case "f32b": return ImageStack.VoxelFormat.FloatBE;
            }

            return def;
        }

        void ParseVector3(string tX, string tY, string tZ, Vector3 def, out Vector3 dest)
        {
            float x, y, z;
            bool valid = true;
            valid &= float.TryParse(tX, out x);
            valid &= float.TryParse(tY, out y);
            valid &= float.TryParse(tZ, out z);

            if (valid) dest = new Vector3(x, y, z);
            else dest = def;
        }
    }
}
