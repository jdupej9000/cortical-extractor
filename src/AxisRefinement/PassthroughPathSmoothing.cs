using CorticalExtract.DataStructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

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
