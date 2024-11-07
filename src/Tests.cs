using CorticalExtract.DataStructures;
using CorticalExtract.Processing;
using NUnit.Framework;
using System;
using System.IO;
using System.Numerics;



namespace CorticalExtract
{
    [TestFixture]
    public class SingleExtractionTests
    {
        [SetUp]
        public void Init()
        {
            fileName = @"E:\data\alize_femur\31M_1980_09_wo_gantry.raw";
            width = 512;
            height = 512;
            slices = 1098;
            voxelFormat = ImageStack.VoxelFormat.FloatBE;
            voxDim = new float[3] { 1, 1, 1 };

            lms = new Vector3[] {new Vector3(129,264,408),
                new Vector3(166,220,1061),
                new Vector3(166,275,487)};

            stk = ImageStack.FromFile(fileName, width, height, slices, voxelFormat, voxDim);
            stk.OffsetAll(-1000);
        }

        [TearDown]
        public void Dispose()
        {
            stk = null;
            GC.Collect();
        }

        string fileName;
        int width, height, slices;
        ImageStack.VoxelFormat voxelFormat;
        float[] voxDim;
        Vector3[] lms;
        ImageStack stk;


        [Test]
        public void LoadSuccessTest()
        {
            for (int k = 0; k < slices; k++)
            {
                for (int j = 0; j < height; j++)
                {
                    for (int i = 0; i < width; i++)
                    {
                        float v = stk[i, j, k];
                        if (v < -1000 | v > 3096)
                            Assert.Fail(string.Format("Invalid HU value. I({0},{1},{2})=({3})", i, j, k, v));
                    }
                }
            }
            Assert.Pass();
        }

        [Test]
        public void BondioliExtractionTest()
        {
            Extractor ext = new Extractor(stk, lms[0], lms[1], lms[2]);
            ext.UseHalfMaxHeight = false;
            ext.Extract(0.2f, 0.8f, 100);

            SaveString(@"D:\profile_Bondioli.csv",
                ext.ThicknessToString());
        }





        protected void SaveString(string file, string content)
        {
            StreamWriter sw = new StreamWriter(new FileStream(file, FileMode.OpenOrCreate));
            sw.Write(content);
            sw.Close();
        }
    }
}
