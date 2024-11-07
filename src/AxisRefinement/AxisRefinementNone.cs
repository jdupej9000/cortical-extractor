using CorticalExtract.DataStructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace CorticalExtract.AxisRefinement
{
    public class AxisRefinementNone : IAxisRefinement
    {
        public AxisRefinementNone()
        {
        }
                
        public Vector3[] Process(ImageStack stk, Vector3 normal, Vector3 binormal, Vector3[] origins)
        {
            return origins;
        }
    }
}
