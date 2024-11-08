using CorticalExtract.DataStructures;

namespace CorticalExtract.Processing
{
    public class ClassificationSetup
    {
        public ClassificationSetup(string configLine)
        {
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
            ParseInt(parts[9], 0, out slice);

            ParseFloat(parts[10], 0, out min0);
            ParseFloat(parts[11], 0, out max0);
            ParseFloat(parts[12], 0, out min1);
            ParseFloat(parts[13], 0, out max1);
            ParseFloat(parts[14], 0, out min2);
            ParseFloat(parts[15], 0, out max2);
        }

        public string pathRaw;
        public int width, height, slices;
        public ImageStack.VoxelFormat format;
        public float[] voxDim;
        public float offset;
        public int slice;
        public float min0, min1, min2, max0, max1, max2;

        static void ParseInt(string text, int def, out int dest)
        {
            if (!int.TryParse(text, out dest))
                dest = def;
        }

        static void ParseFloat(string text, float def, out float dest)
        {
            if (!float.TryParse(text, out dest))
                dest = def;
        }

        static void ParseBool(string text, bool def, out bool dest)
        {
            if (!bool.TryParse(text, out dest))
                dest = def;
        }

        static ImageStack.VoxelFormat ParseFormat(string text, ImageStack.VoxelFormat def)
        {
            return text switch
            {
                "u16" => ImageStack.VoxelFormat.UShortLE,
                "i16" => ImageStack.VoxelFormat.ShortLE,
                "i16b" => ImageStack.VoxelFormat.ShortBE,
                "f32" => ImageStack.VoxelFormat.FloatLE,
                "f32b" => ImageStack.VoxelFormat.FloatBE,
                _ => def
            };
        }
    }
}
