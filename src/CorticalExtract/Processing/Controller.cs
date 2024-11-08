using CorticalExtract.AxisRefinement;
using CorticalExtract.DataStructures;
using CorticalExtract.Forms;
using System;
using System.Collections.Generic;
using System.Formats.Tar;
using System.IO;
using System.Numerics;
using System.Text;
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
            sourcePath = @"D:\file.raw";
            destPath = @"D:\file.csv";
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

                prog(i + 1, numTasks, string.Format("Loading {0}.", Path.GetFileName(setup.pathRaw)));
                ImageStack stk = ImageStack.FromFile(Path.Combine(path, setup.pathRaw),
                    setup.width, setup.height, setup.slices, setup.format);

                prog(i + 1, numTasks, "Applying HU offset.");
                stk.OffsetAll(setup.offset);

                prog(i + 1, numTasks, "Filtering.");
                if (cfg.MedianFilterRadius > 0)
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
                        File.WriteAllText(Path.Combine(destPath, setup.pathDestProfile), ext.ThicknessToString());
                    if (setup.pathDestAxis != string.Empty)
                    {
                        File.WriteAllText(Path.Combine(destPath, setup.pathDestAxis), ext.CentroidsToString());
                        File.WriteAllText(Path.Combine(destPath, setup.pathDestAxis) + ".inner.obj", ext.InnerMesh);
                        File.WriteAllText(Path.Combine(destPath, setup.pathDestAxis) + ".outer.obj", ext.OuterMesh);
                    }
                    if (setup.pathDestSegments != string.Empty)
                        File.WriteAllText(Path.Combine(destPath, setup.pathDestSegments), ext.SegmentAeasToString());
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
    }
}
