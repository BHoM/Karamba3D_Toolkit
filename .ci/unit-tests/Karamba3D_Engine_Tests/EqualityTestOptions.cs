using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Karamba3D_Engine_Tests
{
    public class EqualityTestOptions
    {
        public virtual double DoubleTolerance { get; set; } = 0;
        public virtual float SingleTolerance { get; set; } = 0;

        public virtual decimal DecimalTolerance { get; set; } = 0;
        public virtual string FailureMessage { get; set; } = string.Empty;

        public virtual bool AreTolerancesEnabled => DoubleTolerance != 0 || SingleTolerance != 0 || DecimalTolerance != 0;
    }
}
