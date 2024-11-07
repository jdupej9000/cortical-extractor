using CorticalExtract.DataStructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorticalExtract.AxisRefinement
{
    public class PassthroughPathSmoothing : IPathSmooting
    {
        public PassthroughPathSmoothing()
        {
        }

        public Point3f[] Process(Point3f[] x)
        {
            return x;
        }
    }
}
