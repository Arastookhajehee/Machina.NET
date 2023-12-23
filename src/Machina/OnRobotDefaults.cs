using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Machina
{
    public class OnRobotDefaults
    {
        public static Dictionary<string, int> OnRobotScewTypes = new Dictionary<string, int>
        {
            { "M1.6" , 17 },
            { "M2" , 36 },
            { "M2.5" , 73 },
            { "M3" , 127 },
            { "M4" , 300 },
            { "M5" , 500 },
            { "M6" , 500 },
        };

        public static double tourqueScaleRatio = 100;
    }
}
