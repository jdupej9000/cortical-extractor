using CorticalExtract.DataStructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorticalExtract.AxisRefinement
{
    public interface IPathSmooting
    {
        Point3f[] Process(Point3f[] x);
    }
}
