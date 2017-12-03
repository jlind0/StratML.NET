using System;
using System.Collections.Generic;
using System.Text;
using StratML.Core.One;
using StratML.Core.Two;

namespace StratML.Transform.Core
{
    public interface ITransformOneToTwo
    {
        PerformancePlanOrReport Transform(StrategicPlan plan);
    }
    public interface ITransformTwoToOne
    {
        StrategicPlan Transform(PerformancePlanOrReport plan);
    }
}
