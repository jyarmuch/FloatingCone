using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PitLib
{
    public enum ArcValidation
    {
        Valid,
        FlowExceedsCapacity,
        FlowNegative,
        CapacityNegative,
    }

}
