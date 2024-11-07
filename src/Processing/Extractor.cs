using CorticalExtract.AxisRefinement;
using CorticalExtract.DataStructures;
using System;
using System.Text;

namespace CorticalExtract.Processing
{
    public class Extractor
    {
        public Extractor(ImageStack stk, Point3f lm0, Point3f lm1, Point3f lmd)
            : this(stk, lm0, lm1, lmd, new float[3] { 1, 1, 1 })
        {
        }

        public Extractor(ImageStack stk, Point3f lm0, Point3f lm1, Point3f lmd, float[] voxelDim)
        {
            this.lm0 = lm0;
            this.lm1 = lm1;
            this.lmd = lmd;

            slicer = new Reslicer(stk);

            origStack = stk;
            priorThreshold = DefaultPriorThreshold;
            maxBoneRadius = DefaultMaxBoneRadius;
            AxisRefinement = null;
            numRays = DefaultNumRays;
            useHmh = true;
            caSlices = null;
            axisPoints = null;
            voxelDimensions = voxelDim;
            RefineEndpoints = true;
        }

        ImageStack origStack;
        Point3f lm0, lm1, lmd;
        Reslicer slicer;
        float maxBoneRadius;
        float priorThreshold;
        int numRays;
        bool useHmh;
        float[] voxelDimensions;
        Point2f[,] innerBoundary;
        Point2f[,] outerBoundary;
        float[,] thickness;
        float[,] segmentAreas;
        ImageStack caSlices;
        Point3f[] axisPoints;
        Point3f planeVec0, planeVec1;
        string innerObj, outerObj;

        const float DefaultPriorThreshold = 525;
        const float DefaultMaxBoneRadius = 80;
        const int DefaultNumRays = 50;

        public float MaxBoneRadius
        {
            get { return maxBoneRadius; }
            set { maxBoneRadius = value; }
        }

        public float PriorThreshold
        {
            get { return priorThreshold; }
            set { priorThreshold = value; }
        }

        public int NumRays
        {
            get { return numRays; }
            set { numRays = value; }
        }

        public bool UseHalfMaxHeight
        {
            get { return useHmh; }
            set { useHmh = value; }
        }

        public ImageStack CaSlices
        {
            get { return caSlices; }
        }

        public Point2f[,] InnerBoundary
        {
            get { return innerBoundary; }
        }

        public Point2f[,] OuterBoundary
        {
            get { return outerBoundary; }
        }

        public float[,] Thickness
        {
            get { return thickness; }
        }

        public Point3f[] Centroids
        {
            get { return axisPoints; }
        }

        public Point3f[] PlaneVec
        {
            get { return new Point3f[2] { planeVec0, planeVec1 }; }
        }
    
        public float[,] SegmentAreas
        {
            get { return segmentAreas; }
        }

        public string InnerMesh
        {
            get { return innerObj; }
        }

        public string OuterMesh
        {
            get { return outerObj; }
        }

        public IAxisRefinement AxisRefinement
        {
            get;
            set;
        }

        public bool RefineEndpoints
        {
            get;
            set;
        }

        protected void CalcPlaneVectors(out Point3f normal, out Point3f binormal)
        {
            Point3f u = (lm1 - lm0).Scale(voxelDimensions);
            Point3f v = (lmd - lm0).Scale(voxelDimensions);
            Point3f w = v - u * (Point3f.Dot(v, u) / u.LengthSq);
            Point3f lmp = lmd - w;

            normal = lmd - lmp;
            normal.UnScale(voxelDimensions);
            
            binormal = Point3f.Cross(u, normal);
            binormal.UnScale(voxelDimensions);

            normal.Normalize();
            binormal.Normalize();
            
        }

        protected void ExtractBoundaries(ImageStack stk, Point3f normal, Point3f binormal)
        {
            int n = stk.Slices;

            innerBoundary = new Point2f[n, numRays];
            outerBoundary = new Point2f[n, numRays];
            Point2f center = new Point2f((float)stk.Width / 2, (float)stk.Height / 2);

            //Parallel.For(0, n, (i) =>
            for(int i = 0; i < n; i++)
            {
                BoundarySegmentation seg = new BoundarySegmentation(stk, i, maxBoneRadius);

                float[] radiusInner = new float[n];
                float[] radiusOuter = new float[n];

                float[] radiusInnerImp = new float[n];
                float[] radiusOuterImp = new float[n];

                for (int j = 0; j < numRays; j++)
                {
                    float theta = (float)(2.0 * Math.PI / (float)numRays * j);
                    Point2f direction = new Point2f((float)(Math.Cos(theta)), (float)(Math.Sin(theta)));

                    RayMarchResult t0, t1, t2;

                    float threshOut = priorThreshold;
                    float threshIn = threshOut;//seg.AutoThreshold(center, direction, 25.0f);

                    if (useHmh)
                    {
                        //t0 = seg.HalfMaxHeight(center, direction, 0.0f, threshIn, seg.RayCastRise);
                        //t0 = seg.RayCastRise(center, direction, 0.0f, threshIn);
                        t0 = new RayMarchResult(0);
                        t1 = seg.HalfMaxHeight(center, direction, t0.r + 0.0f, threshOut, seg.RayCastFall);
                        t2 = seg.HalfMaxHeight(center, direction, t1.r - 0.25f, threshIn, seg.RayCastFallReverse, false, 0, 4);
                        //t2 = seg.HalfMaxHeight(center, direction, 0.0f, threshIn+500, seg.RayCastRise);
                    }
                    else
                    {
                        t0 = new RayMarchResult(seg.RayCastRise(center, direction, 0.0f, threshIn));
                        t1 = new RayMarchResult(seg.RayCastFall(center, direction, t0.r + 0.5f, threshOut));
                        t2 = new RayMarchResult(seg.RayCastFallReverse(center, direction, t1.r - 1.0f, threshIn));
                    }

                    //if (t1.r - t2.r < 0.3f) t2.valid = false;
                    radiusInner[j] = t2.valid ? t2.r : float.NaN;
                    radiusOuter[j] = t2.valid ? t1.r : float.NaN;

                }

                for (int j = 0; j < numRays; j++)
                {
                    radiusInnerImp[j] = float.IsNaN(radiusInner[j]) ?
                        Utils.ImputeCyclic(radiusInner, j, 5, radiusOuter[j], false) : radiusInner[j];

                    if (radiusOuter[j] < radiusInnerImp[j]) radiusOuterImp[j] = Utils.ImputeCyclic(radiusOuter, j, 5, radiusInnerImp[j], false);
                    else radiusOuterImp[j] = radiusOuter[j];
                }

                for(int j = 0; j < numRays; j++)
                {
                    float theta = (float)(2.0 * Math.PI / (float)numRays * j);
                    Point2f direction = new Point2f((float)(Math.Cos(theta)), (float)(Math.Sin(theta)));

                    innerBoundary[i, j] = radiusInnerImp[j] * direction;
                    outerBoundary[i, j] = radiusOuterImp[j] * direction;
                }
            }


        }

        public void CalcThickness(Point3f normal, Point3f binormal, float[] voxelDim)
        {
            int n = innerBoundary.GetLength(0);
            thickness = new float[n, numRays];

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < numRays; j++)
                {
                    Point2f outerPt = outerBoundary[i, j];
                    Point2f innerPt = new Point2f(0, 0);
                    float bestDist = float.MaxValue;

                    for (int k = 0; k < numRays; k++)
                    {
                        Point2f a0 = innerBoundary[i, k];
                        Point2f a1 = innerBoundary[i, (k + 1) % numRays];
                        Point2f proj;
                        float dist = Utils.PointSegmentDistance(a0, a1, outerPt, out proj);
                        if (bestDist > dist)
                        {
                            bestDist = dist;
                            innerPt = proj;
                        }
                    }

                    Point3f outerWorld = outerPt.X * normal + outerPt.Y * binormal;
                    Point3f innerWorld = innerPt.X * normal + innerPt.Y * binormal;

                    thickness[i, j] = Point3f.DistanceAniso(outerWorld, innerWorld, voxelDim);
                }
            }
        }

        public void GetMesh(Point3f t0, Point3f t1, out string inner, out string outer)
        {
            inner = BoundaryToMesh(innerBoundary, axisPoints, t0, t1, voxelDimensions);
            outer = BoundaryToMesh(outerBoundary, axisPoints, t0, t1, voxelDimensions);
        }

        public void CalcSegmentAreas(Point3f t0, Point3f t1, float[] voxDim)
        {
            int k = innerBoundary.GetLength(0);
            int m = innerBoundary.GetLength(1);

            float[,] ret = new float[k, 2];

            for (int i = 0; i < k; i++)
            {
                float innerAccum = 0, outerAccum = 0;

                for (int j0 = 0; j0 < m; j0++)
                {
                    int j1 = j0 + 1;
                    if (j1 >= m) j1 = 0;

                    float trIn = Utils.TriangleAreaAniso(new Point2f(0, 0),
                        innerBoundary[i, j0], innerBoundary[i, j1], t0, t1, voxDim);

                    float trOut = Utils.TriangleAreaAniso(new Point2f(0, 0),
                        outerBoundary[i, j0], outerBoundary[i, j1], t0, t1, voxDim);

                    if (!float.IsNaN(trIn)) innerAccum += trIn;
                    if (!float.IsNaN(trOut)) outerAccum += trOut;
                }

                ret[i, 0] = innerAccum;
                ret[i, 1] = outerAccum;
            }

            segmentAreas = ret;
        }

        public void Extract(float t0, float t1, int k)
        {
            Point3f p0 = lm0 + (lm1 - lm0) * t0;
            Point3f p1 = lm0 + (lm1 - lm0) * t1;

            Point3f[] sliceOrigin = Utils.Seq(p0, p1, k);
            Point3f normal, binormal;
            CalcPlaneVectors(out normal, out binormal);
            planeVec0 = normal;
            planeVec1 = binormal;

            caSlices = slicer.Reslice(sliceOrigin, normal, binormal, (int)(maxBoneRadius + 1));

            if (RefineEndpoints)
            {
                AxisRefinementEndpoints are = new AxisRefinementEndpoints(origStack, t0, t1);
                Point3f[] endPts = are.Process(caSlices, normal, binormal, sliceOrigin);

                p0 = endPts[0] + (endPts[1] - endPts[0]) * t0;
                p1 = endPts[0] + (endPts[1] - endPts[0]) * t1;
                sliceOrigin = Utils.Seq(p0, p1, k);
                CalcPlaneVectors(out normal, out binormal);
                planeVec0 = normal;
                planeVec1 = binormal;

                caSlices = slicer.Reslice(sliceOrigin, normal, binormal, (int)(maxBoneRadius + 1));
            }

            if (AxisRefinement != null)
            {
                sliceOrigin = AxisRefinement.Process(caSlices, normal, binormal, sliceOrigin);
                caSlices = slicer.Reslice(sliceOrigin, normal, binormal, (int)(maxBoneRadius + 1));
            }

            axisPoints = sliceOrigin;

            ExtractBoundaries(caSlices, normal, binormal);
            CalcThickness(normal, binormal, voxelDimensions);
            CalcSegmentAreas(normal, binormal, voxelDimensions);
            GetMesh(normal, binormal, out innerObj, out outerObj);
        }

        public string CentroidsToString()
        {
            if (axisPoints == null) return string.Empty;

            StringBuilder sb = new StringBuilder();

            int n = axisPoints.Length;
            for (int i = 0; i < n; i++)
            {
                sb.AppendFormat("{0},{1},{2}",
                    (axisPoints[i].X - axisPoints[n / 2].X) * voxelDimensions[0],
                    (axisPoints[i].Y - axisPoints[n / 2].Y) * voxelDimensions[1],
                    (axisPoints[i].Z - axisPoints[n / 2].Z) * voxelDimensions[2]);
                sb.AppendLine();
            }

            return sb.ToString();
        }

        public string BoundaryToMesh(Point2f[,] bnd, Point3f[] axis, Point3f t0, Point3f t1, float[] voxDim)
        {
            StringBuilder sb = new StringBuilder();

            int slices = bnd.GetLength(0);
            int rays = bnd.GetLength(1);

            for (int i = 0; i < slices; i++)
            {
                for (int j = 0; j < rays; j++)
                {
                    Point3f pt = axis[i] + t0 * bnd[i, j].X + t1 * bnd[i, j].Y - axis[slices/2];
                    sb.AppendLine(string.Format("v {0} {1} {2}",
                        pt.X * voxDim[0], pt.Y * voxDim[1], pt.Z * voxDim[2]));
                }
            }

            for (int i = 0; i < slices - 1; i++)
            {
                for(int j = 0; j < rays; j++)
                {
                    int j1 = j + 1;
                    if (j1 >= rays) j1 = 0;

                    sb.AppendLine(string.Format("f {0} {1} {2}",
                        1 + i * rays + j, 1 + i * rays + j1, 1 + (i + 1) * rays + j));

                    sb.AppendLine(string.Format("f {0} {1} {2}",
                        1 + i * rays + j1, 1 + (i + 1) * rays + j1, 1 + (i + 1) * rays + j));
                }
            }

            return sb.ToString();
        }

        public string ThicknessToString()
        {
            StringBuilder sb = new StringBuilder();

            int n = thickness.GetLength(0);
            int m = thickness.GetLength(1);

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < m - 1; j++)
                    sb.AppendFormat("{0},", thickness[i, j]);

                sb.AppendFormat("{0}", thickness[i, m - 1]);
                sb.AppendLine();
            }

            return sb.ToString();
        }

        public string SegmentAeasToString()
        {
            int rows = segmentAreas.GetLength(0);
            int cols = segmentAreas.GetLength(1);

            return Utils.MatrixToString(rows, cols,
                (i, j) => segmentAreas[i, j]);
        }
    }
}
