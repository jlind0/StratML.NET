using System;
using System.Collections.Generic;
using System.Text;
using StratML.Core.One;
using StratML.Core.Two;
using StratML.Transform.Core;
using StratML.Core;

namespace StratML.Transform
{
    public class TransformOneToTwo : ITransformOneToTwo
    {
        public PerformancePlanOrReport Transform(StrategicPlan plan)
        {
            return plan.CreateRelatedInstance<PerformancePlanOrReport>();
        }
    }
    public class TransformTwoToOne : ITransformTwoToOne
    {
        public StrategicPlan Transform(PerformancePlanOrReport plan)
        {
            return plan.CreateRelatedInstance<StrategicPlan>();
        }
    }
}
