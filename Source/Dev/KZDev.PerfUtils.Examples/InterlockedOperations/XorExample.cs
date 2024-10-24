using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using KZDev.PerfUtils;

namespace KZDev.PerfUtils.Examples
{
    public class XorExample
    {
        private int _flag;

        public bool ToggleFlag ()
        {
            int originalValue = InterlockedOps.Xor(ref _flag, 1);
            return originalValue == 0;
        }
    }
}
