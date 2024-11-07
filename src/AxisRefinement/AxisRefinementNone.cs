using CorticalExtract.DataStructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorticalExtract.AxisRefinement
{
    public class AxisRefinementNone : IAxisRefinement
    {
        public AxisRefinementNone()
        {
        }
                
        public Point3f[] Process(ImageStack stk, Point3f normal, Point3f binormal, Point3f[] origins)
        {
            return origins;
        }
    }
}
