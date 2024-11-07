using CorticalExtract.AxisRefinement;
using CorticalExtract.DataStructures;
using CorticalExtract.Forms;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CorticalExtract.Processing
{
    public class Controller
    {
        public Controller()
        {
            SetDefault();

        }

        string sourcePath, destPath;
        Vector3 landmark0, landmark1, landmarkD;
        int[] voxCount;
        int numSlices, numRays;
        float t0, t1, thresh;
        float[] voxDim;
        string axisFit;
        ImageStack.VoxelFormat voxFmt;
        bool useHmh;
        Configuration cfg = new Configuration();

        public delegate void ProgressDelegate(int task, int numTasks, string comment);


        public Configuration Config
        {
            get { return cfg; }
        }

        public void SetDefault()
        {
            sourcePath = @"D:\32M.raw";
            destPath = @"D:\32M.csv";
            voxCount = new int[3] { 512, 512, 937 };
            voxDim = new float[3] { 1, 1, 1 };
            voxFmt = ImageStack.VoxelFormat.FloatBE;
            landmark0 = Vector3.Zero;
            landmark1 = Vector3.Zero;
            landmarkD = Vector3.Zero;

            numSlices = 100;
            numRays = 50;
            t0 = 0.2f;
            t1 = 0.8f;
            thresh = 250f;
            axisFit = "none";
            useHmh = true;
        }

        public void ExecuteClassificationScript(string fileName, string destFile, bool debugEach, ProgressDelegate prog)
        {
            prog(0, 0, "Loading script.");
            string path = Path.GetDirectoryName(fileName);
            List<string> lines = new List<string>();
            StreamReader sr = new StreamReader(new FileStream(fileName, FileMode.Open, FileAccess.Read));
            while (!sr.EndOfStream) lines.Add(sr.ReadLine());
            sr.Close();

            StreamWriter sw = new StreamWriter(new FileStream(destFile, FileMode.OpenOrCreate, FileAccess.ReadWrite));
            sw.WriteLine("file.name,count0,area0,count1,area1,count2,area2");

            int numTasks = lines.Count;
            for (int i = 0; i < numTasks; i++)
            {
                ClassificationSetup setup = new ClassificationSetup(lines[i]);

                prog(i + 1, numTasks, string.Format("Loading {0}.", Path.GetFileName(setup.pathRaw)));
                ImageStack stk = ImageStack.FromFile(Path.Combine(path, setup.pathRaw),
                    setup.width, setup.height, setup.slices, setup.format,
                    new float[3] { 1, 1, 1 },
                    new int[5] { setup.slice - 3, setup.slice - 2, setup.slice - 1, setup.slice, setup.slice + 1 });

                prog(i + 1, numTasks, "Applying HU offset.");
                stk.OffsetAll(setup.offset);

                prog(i + 1, numTasks, "Filtering.");
                SliceMedianFilter median = new SliceMedianFilter(2);
                ImageStack filtered = median.Process(stk);

                prog(i + 1, numTasks, "Classifying.");
                ThresholdClassification cls = new ThresholdClassification();
                cls.AddRange(1, new ThresholdClassification.ThresholdRange(setup.min0, setup.max0));
                cls.AddRange(2, new ThresholdClassification.ThresholdRange(setup.min1, setup.max1));
                cls.AddRange(3, new ThresholdClassification.ThresholdRange(setup.min2, setup.max2));
                byte[] mask = cls.GetMaskSlice(filtered, 0);
                cls.AccumulateMask(mask);

                StringBuilder sb = new StringBuilder();
                sb.AppendFormat("{0},", setup.pathRaw);
                sb.AppendFormat("{0},", cls.GetAccumCount(1));
                sb.AppendFormat("{0},", (float)cls.GetAccumCount(1) * setup.voxDim[0] * setup.voxDim[1] / 100);
                sb.AppendFormat("{0},", cls.GetAccumCount(2));
                sb.AppendFormat("{0},", (float)cls.GetAccumCount(2) * setup.voxDim[0] * setup.voxDim[1] / 100);
                sb.AppendFormat("{0},", cls.GetAccumCount(3));
                sb.AppendFormat("{0}", (float)cls.GetAccumCount(3) * setup.voxDim[0] * setup.voxDim[1] / 100);

                sw.WriteLine(sb.ToString());

                if (cfg.DebugAll)
                {
                    DebugForm frm = new DebugForm();
                    frm.Items.Add(new StackViewItem("Original substack", stk));
                    frm.Items.Add(new StackViewItem("Filtered", filtered));
                    frm.Items.Add(new StackViewItem("Filtered and classified", filtered, mask));
                    frm.ShowDialog();
                }
            }

            sw.Close();
        }

        public void ExecuteScript(string fileName, List<ExtractorSetup> tasks, bool debugEach, ProgressDelegate prog, bool debugMode = false)
        {        
            string path = Path.GetDirectoryName(fileName);
            string destPath = Path.Combine(path, Config.ResultPath);
            Directory.CreateDirectory(destPath);

            int numTasks = tasks.Count;

            for (int i = 0; i < numTasks; i++)            
            {
                ExtractorSetup setup = tasks[i];

                prog(i+1, numTasks, string.Format("Loading {0}.", Path.GetFileName(setup.pathRaw)));
                ImageStack stk = ImageStack.FromFile(Path.Combine(path, setup.pathRaw),
                    setup.width, setup.height, setup.slices, setup.format);

                prog(i + 1, numTasks, "Applying HU offset.");
                stk.OffsetAll(setup.offset);

                prog(i + 1, numTasks, "Filtering.");
                if(cfg.MedianFilterRadius > 0)
                    stk = Filtering.MedianFilter(stk, cfg.MedianFilterRadius);

                prog(i + 1, numTasks, "Extracting CA profile.");
                Extractor ext = new Extractor(stk, setup.lm0, setup.lm1, setup.lmd, setup.voxDim);
                ext.UseHalfMaxHeight = cfg.UseHmh;
                ext.AxisRefinement = cfg.GetAxisRefinement();
                ext.RefineEndpoints = cfg.RefineEndpoints;
                ext.PriorThreshold = cfg.PriorThreshold;
                ext.NumRays = cfg.NumRays;
                ext.MaxBoneRadius = cfg.MaxBoneRadius;
                ext.Extract(cfg.BoneRoiStart, cfg.BoneRoiEnd, cfg.NumSlices);

                if (!debugMode)
                {
                    prog(i + 1, numTasks, "Saving.");
                    if (setup.pathDestProfile != string.Empty)
                        SaveString(Path.Combine(destPath, setup.pathDestProfile), ext.ThicknessToString());
                    if (setup.pathDestAxis != string.Empty)
                    {
                        SaveString(Path.Combine(destPath, setup.pathDestAxis), ext.CentroidsToString());
                        SaveString(Path.Combine(destPath, setup.pathDestAxis) + ".inner.obj", ext.InnerMesh);
                        SaveString(Path.Combine(destPath, setup.pathDestAxis) + ".outer.obj", ext.OuterMesh);
                    }
                    if (setup.pathDestSegments != string.Empty)
                        SaveString(Path.Combine(destPath, setup.pathDestSegments), ext.SegmentAeasToString());
                }

                if (cfg.DebugAll | debugMode)
                {
                    DebugForm frm = new DebugForm();
                    frm.Items.Add(new StackViewItem("Original stack", stk, setup.voxDim));

                    StackViewItem sviResliced = new StackViewItem("Resliced bone", ext.CaSlices, ext.InnerBoundary, ext.OuterBoundary);
                    sviResliced.center = ext.Centroids;
                    sviResliced.t0 = ext.PlaneVec[0];
                    sviResliced.t1 = ext.PlaneVec[1];
                    frm.Items.Add(sviResliced);

                    frm.Items.Add(new StackViewItem("Thickness profile", new ImageStack(ext.Thickness)));
                    frm.ShowDialog();
                }

                stk = null;
                GC.Collect();
            }

            prog(0, 0, "All done.");
        }

        public IAxisRefinement AxisRefinementByParam(string mode, float param)
        {
            switch (mode)
            {
                case "none":
                    return new AxisRefinementNone();

                case "passthrough":
                    return new AxisRefinementCrossCentroids(new PassthroughPathSmoothing());

                case "linear":
                    return new AxisRefinementCrossCentroids(new LinearPathSmoothing(), true);

                case "gaussian":
                    return new AxisRefinementCrossCentroids(new GaussianPathSmoothing(param));
            }

            return null;
        }

        public void RunBatchWithDialog()
        {
            BatchForm dlg = new BatchForm();
            dlg.controller = this;
            //dlg.ShowDialog();

        }

        public void SimpleTest()
        {
            string fileName;
            int width, height, slices;
            ImageStack.VoxelFormat voxelFormat;
            float[] voxDim;
            Vector3[] lms;
            ImageStack stk;

            fileName = @"E:\Saruman\data\alize\31M_1980_09_wo_gantry.raw";
            width = 512;
            height = 512;
            slices = 1098;
            voxelFormat = ImageStack.VoxelFormat.FloatBE;
            voxDim = new float[3] { 0.9766f, 0.9766f, 0.9766f };

            lms = new Vector3[] {new Vector3(406,241,399),
                new Vector3(365,244,1062),
                new Vector3(368,262,472)};

            stk = ImageStack.FromFile(fileName, width, height, slices, voxelFormat);
            stk.OffsetAll(-1000);

            Extractor ext = new Extractor(stk, lms[0], lms[1], lms[2], voxDim);
            ext.AxisRefinement = new AxisRefinementCrossCentroids(new GaussianPathSmoothing(3));
            ext.UseHalfMaxHeight = true;
            ext.Extract(0.2f, 0.8f, 100);

            DebugForm frm = new DebugForm();
            frm.Items.Add(new StackViewItem("Original stack", stk));
            frm.Items.Add(new StackViewItem("Resliced bone", ext.CaSlices, ext.InnerBoundary, ext.OuterBoundary));

            frm.ShowDialog();

            SaveString(@"D:\profile_Bondioli.csv",
                ext.ThicknessToString());

            SaveString(@"D:\axis_Bondioli.csv",
                ext.CentroidsToString());
        }

        public void ClassificationTest()
        {
            string fileName;
            int width, height, slices;
            ImageStack.VoxelFormat voxelFormat;
            float[] voxDim;
            ImageStack stk;

            fileName = @"E:\Saruman\data\alize\31M_1980_09_wo_gantry.raw";
            width = 512;
            height = 512;
            slices = 1098;
            voxelFormat = ImageStack.VoxelFormat.FloatBE;
            voxDim = new float[3] { 0.9766f, 0.9766f, 0.9766f };
            int[] sliceSel = new int[3] { 170, 171, 172 };

            stk = ImageStack.FromFile(fileName, width, height, slices, voxelFormat, voxDim, sliceSel);
            stk.OffsetAll(-1000);
            SliceMedianFilter median = new SliceMedianFilter(1);
            ImageStack filtered = median.Process(stk);

            ThresholdClassification cls = new ThresholdClassification();
            cls.AddRange(1, new ThresholdClassification.ThresholdRange(-205, 32));
            cls.AddRange(2, new ThresholdClassification.ThresholdRange(32, 147));
            cls.AddRange(3, new ThresholdClassification.ThresholdRange(147, 3000));
            byte[] mask = cls.GetMaskSlice(filtered, 0);
            cls.AccumulateMask(mask);

            string report = string.Format("n0={0}, n1={1}, n2={2}",
                cls.GetAccumCount(0), cls.GetAccumCount(1), cls.GetAccumCount(2));

            DebugForm frm = new DebugForm();
            frm.Items.Add(new StackViewItem("Original substack", stk));
            frm.Items.Add(new StackViewItem("Filtered", filtered));
            frm.Items.Add(new StackViewItem("Filtered and classified", filtered, mask));

            frm.ShowDialog();
            MessageBox.Show(report);

        }

        protected void SaveString(string file, string content)
        {
            StreamWriter sw = new StreamWriter(new FileStream(file, FileMode.Create));
            sw.Write(content);
            sw.Close();
        }
    }
}
