using CorticalExtract.DataStructures;
using System.Numerics;

namespace CorticalExtract.AxisRefinement
{
    public interface IAxisRefinement
    {
        Vector3[] Process(ImageStack stk, Vector3 normal, Vector3 binormal, Vector3[] origins);
    }
}
