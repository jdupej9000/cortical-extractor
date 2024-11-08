using System.Numerics;

namespace CorticalExtract.AxisRefinement
{
    public interface IPathSmooting
    {
        Vector3[] Process(Vector3[] x);
    }
}
