using System.Numerics;

namespace CorticalExtract.AxisRefinement
{
    public class PassthroughPathSmoothing : IPathSmooting
    {
        public PassthroughPathSmoothing()
        {
        }

        public Vector3[] Process(Vector3[] x)
        {
            return x;
        }
    }
}
