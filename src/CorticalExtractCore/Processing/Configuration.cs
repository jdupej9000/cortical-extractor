using CorticalExtract.AxisRefinement;
using System.ComponentModel;

namespace CorticalExtract.Processing
{
    public class Configuration
    {
        public Configuration()
        {
            ResultPath = "out";
            UseHmh = true;
            RefineEndpoints = false;
            MedianFilterRadius = 1;
            PriorThreshold = 525;
            AxisDetection = AxisMode.CrossSectionGaussian;
            AxisGaussianBandwidth = 3.0f;
            NumRays = 50;
            NumSlices = 100;
            DebugAll = false;
            BoneRoiStart = 0.2f;
            BoneRoiEnd = 0.8f;
            MaxBoneRadius = 60.0f;
        }

        public enum AxisMode
        {
            Landmarks = 0,
            CrossSectionCentroids = 1,
            CrossSectionLinear = 2,
            CrossSectionGaussian = 3
        }


        [Description("Relative path to results. If directory does not exist, it shall be created.")]
        [DefaultValue("out")]
        public string ResultPath
        {
            get;
            set;
        }

        [Description("Launches a debug window with results for every individual. Note that execution is blocked until user closes that window.")]
        [DefaultValue(false)]
        public bool DebugAll
        {
            get;
            set;
        }

        [Description("Sets whether half-max-height is to be used for segmentation.")]
        [DefaultValue(true)]
        public bool UseHmh
        {
            get;
            set;
        }

        [Description("If true, endpoint refinement is performed.")]
        [DefaultValue(false)]
        public bool RefineEndpoints
        {
            get;
            set;
        }

        [Description("Sets the radius of median filtering (applies to Z-axis as well). If 0, no filtering is applied.")]
        [DefaultValue(1)]
        public int MedianFilterRadius
        {
            get;
            set;
        }

        [Description("The prior threshold for segmentation.")]
        [DefaultValue(525)]
        public int PriorThreshold
        {
            get;
            set;
        }

        [Description("Specifies how the medial axis is calculated.")]
        [DefaultValue(AxisMode.CrossSectionGaussian)]
        public AxisMode AxisDetection
        {
            get;
            set;
        }

        [Description("Specifies the width of smoothing kernel for axis detection. Applicable only to AxisDetection=CrossSectionGaussian.")]
        [DefaultValue(3.0f)]
        public float AxisGaussianBandwidth
        {
            get;
            set;
        }

        [Description("Radius (in pixels) around the landmark-specified axis, where the volume will be searched for the cortical bone.")]
        [DefaultValue(60.0f)]
        public float MaxBoneRadius
        {
            get;
            set;
        }

        [Description("The number of angularly equidistant directions to be sampled.")]
        [DefaultValue(50)]
        public int NumRays
        {
            get;
            set;
        }

        [Description("The number of reslices to be produced.")]
        [DefaultValue(100)]
        public int NumSlices
        {
            get;
            set;
        }

        [Description("Where to start extracting the reslices, along the axis.")]
        [DefaultValue(0.2f)]
        public float BoneRoiStart
        {
            get;
            set;
        }

        [Description("Where to end extracting the reslices, along the axis.")]
        [DefaultValue(0.8f)]
        public float BoneRoiEnd
        {
            get;
            set;
        }

        public IAxisRefinement GetAxisRefinement()
        {
            return AxisDetection switch
            {
                AxisMode.Landmarks => new AxisRefinementNone(),
                AxisMode.CrossSectionCentroids => new AxisRefinementCrossCentroids(new PassthroughPathSmoothing()),
                AxisMode.CrossSectionLinear => new AxisRefinementCrossCentroids(new LinearPathSmoothing(), true),
                AxisMode.CrossSectionGaussian => new AxisRefinementCrossCentroids(new GaussianPathSmoothing(AxisGaussianBandwidth)),
                _ => throw new InvalidOperationException()
            };
        }
    }
}
